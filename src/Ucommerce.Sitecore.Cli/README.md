## How to test
When implementing or changing a step, in order to test it, here's what you could do:
- Go to a command in Ucommerce.Sitecore.Cli 'Commands' folder.
- Comment out the logic in the 'ExecuteAsync' method.
- Implement the creation and running of your step(s).
- Build a folder structure similar to what your step needs.
- If your steps need files to move, copy or otherwise, create some empty files that match the naming.
- Call the command with parameters pointing to the folder structure you made.
- Verify that changes were made by the step.