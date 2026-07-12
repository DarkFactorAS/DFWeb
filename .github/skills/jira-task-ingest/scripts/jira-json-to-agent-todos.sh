#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 1 ]]; then
  echo "Usage: $0 <JIRA_JSON_FILE> [OUTPUT_JSON_FILE]" >&2
  exit 1
fi

if ! command -v jq >/dev/null 2>&1; then
  echo "Missing dependency: jq" >&2
  echo "Install jq and retry." >&2
  exit 3
fi

INPUT_FILE="$1"
OUTPUT_FILE="${2:-}"

if [[ ! -f "${INPUT_FILE}" ]]; then
  echo "Input file not found: ${INPUT_FILE}" >&2
  exit 2
fi

issue_key="$(jq -r '.issue.key // "UNKNOWN-ISSUE"' "${INPUT_FILE}")"
summary="$(jq -r '.issue.fields.summary // "Jira issue"' "${INPUT_FILE}")"

subtasks_json="$(jq -c '[.issue.fields.subtasks[]? | {title: (.fields.summary // .key // "Review subtask")} ]' "${INPUT_FILE}")"

todos_json="$(jq -n \
  --arg issue_key "${issue_key}" \
  --arg summary "${summary}" \
  --argjson subtasks "${subtasks_json}" '
  def base:
    [
      { id: 1, title: ("Review scope for " + $issue_key), status: "not-started" },
      { id: 2, title: ("Implement core changes for " + $issue_key), status: "not-started" },
      { id: 3, title: ("Handle details for: " + $summary), status: "not-started" },
      { id: 4, title: ("Add or update tests"), status: "not-started" },
      { id: 5, title: ("Validate acceptance criteria"), status: "not-started" }
    ];

  def subtask_items:
    $subtasks
    | to_entries
    | map({
        id: (6 + .key),
        title: ("Complete subtask: " + .value.title),
        status: "not-started"
      });

  base + subtask_items
')"

if [[ -n "${OUTPUT_FILE}" ]]; then
  printf '%s\n' "${todos_json}" > "${OUTPUT_FILE}"
  echo "Saved agent todo JSON to ${OUTPUT_FILE}" >&2
else
  printf '%s\n' "${todos_json}"
fi
