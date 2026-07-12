#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 1 ]]; then
  echo "Usage: $0 <ISSUE_KEY> [OUTPUT_JSON_FILE]" >&2
  exit 1
fi

ISSUE_KEY="$1"
OUTPUT_FILE="${2:-}"

required_vars=(JIRA_BASE_URL JIRA_EMAIL JIRA_API_TOKEN)
for var_name in "${required_vars[@]}"; do
  if [[ -z "${!var_name:-}" ]]; then
    echo "Missing required environment variable: ${var_name}" >&2
    exit 2
  fi
done

ISSUE_URL="${JIRA_BASE_URL%/}/rest/api/3/issue/${ISSUE_KEY}?fields=summary,description,issuetype,priority,status,labels,components,assignee,reporter,parent,subtasks,issuelinks"
COMMENT_URL="${JIRA_BASE_URL%/}/rest/api/3/issue/${ISSUE_KEY}/comment?maxResults=20"

issue_json="$(curl -sS -f -u "${JIRA_EMAIL}:${JIRA_API_TOKEN}" -H "Accept: application/json" "${ISSUE_URL}")"
comment_json="$(curl -sS -f -u "${JIRA_EMAIL}:${JIRA_API_TOKEN}" -H "Accept: application/json" "${COMMENT_URL}")"

combined_json="$(cat <<EOF
{
  "issue": ${issue_json},
  "comments": ${comment_json}
}
EOF
)"

if [[ -n "${OUTPUT_FILE}" ]]; then
  printf '%s\n' "${combined_json}" > "${OUTPUT_FILE}"
  echo "Saved Jira payload to ${OUTPUT_FILE}" >&2
else
  printf '%s\n' "${combined_json}"
fi
