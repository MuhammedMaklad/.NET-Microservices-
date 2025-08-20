# Endpoints

| Action                        | Verb | Path                                               | Controller |
| ----------------------------- | ---- | -------------------------------------------------- | ---------- |
| get all platforms             | GET  | /api/c/platforms                                   | Platform   |
| get all commands for platform | GET  | /api/c/platforms/{platformId}/commands             | Command    |
| Get a Command for a platform  | GET  | /api/c/platforms/{platformId}/commands/{commandId} | Command    |
| Create a Command for Platform | POST | /api/c/platforms/{platformId}/commands/            | Command    |
