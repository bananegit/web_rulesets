### Setup

- In SSE go to Policy -> Web Policy -> Policy
- Click on "New Ruleset" in the top right corner and select "Custom Ruleset"
- Set the Ruleset name to whatever you like e.g. MS Copilot Controls
- Select Criteria:
  URL is "copilot.cloud.microsoft"
  OR
  URL is "substrate.office.com"
  OR
  URL matches ```^https:\/\/graph\.microsoft\.com\/v1\.0\/me\/drive\/special\/copilotuploads:\/[^\/]*\/createUploadSession$```
  OR
  URL matches ```^https:\/\/outlook\.office\.com\/hosted\/semanticoverview\/Users\('OID:[^']+?'\).*$```
  OR
  URL matches ```^https:\/\/[^.]*\.resources\.office\.net\/.*\/(?:CloudFilePickerDialog\.html|taskpane\.html)$```
  OR
  URL matches ```^https:\/\/[^.]*\.loki\.delve\.office\.com\/api\/v2\/graphql.*operationName=useLokiFileUploadMutation$```
- Click on "Applies to: All" and unselect "Embedded Objects"
- To import a Rule from this ruleset click on the 3-dot Menu of your ruleset and select "Add Custom Rule" - "Via Policy Code"
- Name the rule like the github file you want to import
- Click on "Edit" and copy and paste the code of the rule into the policy code view
- Click on "Save" to save your rule
  
### Features

This ruleset provides rules for the following actions for the Gemini Webservice:

- Block prompts exceeding length limit (client-side) (Web, InApp)
- Block File Uploads (Web, InApp)
- Block File Uploads from Drive (Web, InApp via FilePicker)
- Block public Link Sharing (Web)
- Block Org-Internal Link Sharing (Web)
- Prevent pasting from clipboard (Web, InApp)
- Block microphone access (Web, InApp)

<br/><br/>

### Limitations

In some contexts file upload (for images) and file selection from drive go through a websocket rather than via api, these can not be blocked with this ruleset
So far this ruleset has been tested in the following contexts, depending on how copilot is integrated it might or might not apply in other contexts:

Web:
copilot.cloud.microsoft
copilot.microsoft.com
m365.cloud.microsoft/chat

InApp:
Word Online
Excel Online
Powerpoint Online
Teams Online (Web rules apply, not the InApp ones)

### Customizing Settings:


Rule: Block prompts exceeding length limit (Web/InApp)

Adjust variable "maxPromptLength" based on your requirements



Rule: Block Entity Uploads from Drive (Web)

Adjust variable "blockedEntities" based on your requirements, available entities are File, Message (Emails), People (Contacts), Event (Calendar entries)

