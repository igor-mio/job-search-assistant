# Contributing

This repository follows a lightweight workflow suitable for a solo project with a clear history and recruiter-friendly commits.

## Branching
- `main`: stable branch
- `develop`: integration branch (optional usage)

Recommended branch naming:
- `feature/<short-name>`
- `fix/<short-name>`
- `docs/<short-name>`
- `chore/<short-name>`

## Commit message convention (Conventional Commits)
Use the following format:

`<type>: <short summary>`

Examples:
- `feat: create applications endpoints`
- `fix: handle null closingDate in response likelihood`
- `docs: update architecture section`
- `chore: configure linting`
- `refactor: extract auth middleware`
- `test: add unit tests for likelihood rules`

### Allowed types
- `feat`     New feature
- `fix`      Bug fix
- `docs`     Documentation only changes
- `chore`    Tooling, config, or maintenance tasks
- `refactor` Code change that neither fixes a bug nor adds a feature
- `test`     Adding or updating tests

## Pull requests (optional)
If using PRs:
- Keep PRs small and focused.
- Link the related EPIC/issue in the description.
