<div id="top"></div>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links

https://www.opensourceagenda.com/projects/the-documentation-compendium
https://www.writethedocs.org/guide/writing/beginners-guide-to-docs/#why-write-docs 

-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]




<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/knightofnet/StartAs">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">Start App As</h3>

  <p align="center">
    Run application with administrator privileges from a standard account, by saving credentials into an encrypted file.
    <br />
    <a href="https://github.com/knightofnet/StartAs"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/knightofnet/StartAs">View Demo</a>
    ·
    <a href="https://github.com/knightofnet/StartAs/issues">Report Bug</a>
    ·
    <a href="https://github.com/knightofnet/StartAs/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
        <li><a href="#potential-security-threat">Potential security threat</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://example.com)

"Start As" is an application for Windows designed to allow launching applications as another profile with administrator privileges from a standard account. For this purpose, the credentials of the administrator account are saved in an encrypted file.

This application is an alternative to an already existing one, but open-source, free and without annoying windows at launch.

<p align="right">(<a href="#top">back to top</a>)</p>

### Built With

* [AryxDevLibrary (by me)](https://www.nuget.org/packages/AryxDevLibrary/)

<p align="right">(<a href="#top">back to top</a>)</p>

### How it works

In some circumstances it can be useful to start an application as another user. This is possible by using the command "runas" in the command prompt. If, for example, I want to start the application "notepad" as "Max", you would write this:

```
runas /user:Max "notepad.exe"
``` 

You will then be prompted to enter the password for Max's session. If you want to enter the password in the command, then you should write this:

```
runas /user:Max /savecreds "notepad.exe"
``` 

You will again be asked to enter the password for Max's session, but then this password is saved with your session. You will be able to check it by going to the "Windows credentials" section of the Windows Credentials Manager (also see [this](https://www.sevenforums.com/tutorials/135805-credential-manager-shortcut-create.html)).

The fact that the password is saved is a security issue: anyone can potentially get that password and thus access to a session with higher privileges. It is to prevent this that "Start As" exists.

By saving the password in a file (named "authentification file", a file of type crt), in an encrypted and secure way, "Start As" allows to share this file with another user so that he can start the desired target program, without revealing the password. In the details, you will first have to create an authentication file using the 'configSaveAs' tool. Then, once this file is created, it will be necessary to pass it as a parameter of the executable "saveAs.exe".


### Potential security threat

For security reasons, it is important that regular Windows users use a session with limited rights, as well as a session with administrator rights. Indeed, for a regularly use, a standard user does not need administrator rights and can ask for an elevation of privileges during installations. Linux, Android or iOs distributions work on the same principle. This precaution won't prevent or mitigate all malware infections. Some malware can "escalate" its system privileges and give itself powers that a limited user doesn't have. But regular, run-of-the-mill malware, which is what most people face most of the time, doesn't do that.

Read this article for more details : [Protect your computer with this one simple trick, TomsGuide.com, Paul Wagenseil, published March 20, 2019](https://www.tomsguide.com/us/limited-account-benefits,news-25682.html).

Based on this, "Start As" can make it easier to start applications as an administrator. Using a web browser can be risky in these conditions: viruses, malware or other security risks are more likely to corrupt the use of your computer. Preferably run browsers with a limited account.

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

This application runs on Microsoft Windows with .net Framework 4.8.

To test that you have the minimum version required, you can run this Powershell command:

1. Open Powershell by typing ```powershell``` into command prompt, or start menu.
2. Write the text above and valid with return :

```
(Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full").Release -ge 528040
```

3. If you can see ```True```, then everything is OK. Else, download and install .net Framework 4.8 by visiting this site : [download .Net Framework 4.8 Runtime (web-installer)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer).

### Installation

1. Get a free API Key at [https://example.com](https://example.com)
2. Clone the repo
   ```sh
   git clone https://github.com/knightofnet/StartAs.git
   ```
3. Install NPM packages
   ```sh
   npm install
   ```
4. Enter your API in `config.js`
   ```js
   const API_KEY = 'ENTER YOUR API';
   ```

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

Use this space to show useful examples of how a project can be used. Additional screenshots, code examples and demos work well in this space. You may also link to more resources.

_For more examples, please refer to the [Documentation](https://example.com)_

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap

- [] Feature 1
- [] Feature 2
- [] Feature 3
    - [] Nested Feature

See the [open issues](https://github.com/knightofnet/StartAs/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Your Name - [@twitter_handle](https://twitter.com/twitter_handle) - email@email_client.com

Project Link: [https://github.com/knightofnet/StartAs](https://github.com/knightofnet/StartAs)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* []()
* []()
* []()

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/knightofnet/StartAs.svg?style=for-the-badge
[contributors-url]: https://github.com/knightofnet/StartAs/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/knightofnet/StartAs.svg?style=for-the-badge
[forks-url]: https://github.com/knightofnet/StartAs/network/members
[stars-shield]: https://img.shields.io/github/stars/knightofnet/StartAs.svg?style=for-the-badge
[stars-url]: https://github.com/knightofnet/StartAs/stargazers
[issues-shield]: https://img.shields.io/github/issues/knightofnet/StartAs.svg?style=for-the-badge
[issues-url]: https://github.com/knightofnet/StartAs/issues
[license-shield]: https://img.shields.io/github/license/knightofnet/StartAs.svg?style=for-the-badge
[license-url]: https://github.com/knightofnet/StartAs/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/linkedin_username
[product-screenshot]: images/screenshot.png
