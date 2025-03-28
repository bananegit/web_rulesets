This ruleset provides rules for the following actions for the DeepSeek Webservice and API:

- Limit prompt Length to x characters by cutting off prompt (Web & API)
- Limit prompt Length by replacing prompt with policy warning when max length is exceeded (Web & API)
- Enforce DeepThink settings (Web)
- Enforce Search settings (Web)
- Block File Uploads (Web)

Customizing Settings:

Rule: Limit prompt length (WebApp)

Adjust variable "maxPromptLength" based on your requirements


Rule: Replace prompt (WebApp)

Adjust variable "maxPromptLength" and "policyWarning" based on your requirements


Rule: Enforce Deepthink Setting (WebApp)

Set the variable "deepThink" to "TRUE" or "FALSE" based on your requirements


Rule: Enforce Search Setting (WebApp)

Set the variable "search" to "TRUE" or "FALSE" based on your requirements


Rule: Limit prompt length (API)

Adjust variable "maxPromptLength" based on your requirements

Remove entries for roles for that you do NOT want to limit the prompt length for from the "limitedRoles" variable


Rule: Replace prompt (API)

Adjust variable "maxPromptLength" based on your requirements

Remove entries for roles for that you do NOT want to replace the prompt for from the "limitedRoles" variable

