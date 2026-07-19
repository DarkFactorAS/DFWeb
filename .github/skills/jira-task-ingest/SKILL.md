---
name: jira-task-ingest
description: 'Log into Jira (via API token), fetch Jira issue details, and create structured agent task lists. Use when asked to read Jira tasks, break requirements into implementation work, or generate execution plans from Jira tickets.'
argument-hint: 'JIRA-123'
user-invocable: true
---

# Jira Task Ingest

Use this skill when the user wants to pull details from a Jira issue and turn those details into concrete coding tasks for the agent.

## When To Use
- User asks to log into Jira and read a ticket.
- User asks to convert Jira issue requirements into a coding plan.
- User asks to generate agent todo items from Jira acceptance criteria.

## Required Inputs
- Jira base URL, for example https://your-company.atlassian.net
- Jira account email
- Jira API token
- Jira issue key, for example JIRA-123
- jq (required by ./scripts/jira-json-to-agent-todos.sh)

Set credentials as environment variables in terminal:

```bash
export JIRA_BASE_URL="https://your-company.atlassian.net"
export JIRA_EMAIL="your-email@company.com"
export JIRA_API_TOKEN="your-api-token"
```

## Procedure
1. Verify required environment variables exist.
2. Fetch the Jira issue JSON with [fetch-jira-issue.sh](./scripts/fetch-jira-issue.sh).
3. Extract implementation signals from the payload:
   - Summary, description, acceptance criteria, links, subtasks
   - Priority, status, labels, components
4. Convert fetched JSON into importable agent todos with [jira-json-to-agent-todos.sh](./scripts/jira-json-to-agent-todos.sh).
5. Refine generated tasks using [agent-task-template.md](./assets/agent-task-template.md) when needed.
6. Open or update the todo list with concise, action-oriented task titles.

## Quick Commands
```bash
./.github/skills/jira-task-ingest/scripts/fetch-jira-issue.sh JIRA-123 jira.json
./.github/skills/jira-task-ingest/scripts/jira-json-to-agent-todos.sh jira.json agent-todos.json
```

## Output Contract
Produce:
- A short issue summary
- Assumptions and open questions
- A prioritized task list suitable for direct execution by the coding agent

## Safety And Secrets
- Never print API tokens in chat output.
- Keep credentials in environment variables only.
- If authentication fails, ask the user to refresh token or verify Jira permissions.

## References
- [Jira API notes](./references/jira-api.md)
