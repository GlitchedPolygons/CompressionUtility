# Building the API Documentation Site
---
* Make sure that you have [DocFX](https://github.com/dotnet/docfx) installed and added to the system's PATH environment variable.
* Open the command prompt inside the `docfx_project` folder.
* Run the following command: `docfx ./docfx.json`
* * You can also achieve the same by running `bash build-docs.sh` in the repository's root directory.
* The compiled output will be deposited here into this folder, which you can then serve either on- or offline for the consumers of this API to navigate.
* * [GitHub Pages](https://help.github.com/en/articles/configuring-a-publishing-source-for-github-pages) provides a phenomenal infrastructure for this purpose. Your DocFX output will be recognized by GitHub automatically if you configure Pages to serve your `master` branch's `docs/` folder.
