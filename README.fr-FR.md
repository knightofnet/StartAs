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
    <img src="reposElements\icon.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">Start App As</h3>

  <p align="center">
    Démarrer une application avec les droits administrateur à partir d'un compte standard, en sauvegardant les informations d'identifications dans un fichier chiffré.
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
  <summary>Sommaire</summary>
  <ol>
    <li>
      <a href="#about-the-project">A propos</a>
      <ul>
        <li><a href="#built-with">Contruit avec</a></li>
        <li><a href="#potential-security-threat">Menace potentielle liée à la sécurité</a></li>
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
      <ul>
        <li><a href="#create-an-authentification-file">Create an authentification file</a></li>
        <li><a href="#installation">Installation</a></li>        
      </ul>    
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://example.com)

"Start As" est une application Windows conçue pour permettre de lancer des applications en tant qu'un autre utilisateur, tel qu'un utilisateur avec des privilèges d'administrateur, à partir d'un compte standard. Les identifiants sont enregistrés dans un fichier chiffré.

Cette application est une alternative à une autre application déjà existante, mais "Start As" est open-source, gratuite et sans fenêtres gênantes au lancement.

<p align="right">(<a href="#top">back to top</a>)</p>

### Built With

* [AryxDevLibrary (by me)](https://www.nuget.org/packages/AryxDevLibrary/)

<p align="right">(<a href="#top">back to top</a>)</p>

### How it works

Dans certains situations il peut être utile de démarrer une application en tant qu'un autre utilisateur (pour des mise à jour de pilotes, l'installation d'un logiciel particulier, etc.). Dans Windows, ceci est possible en utilisant la commande "runas" dans l'invite de commandes. Par exemple, si vous souhaitez démarrer l'applicatin "notepad" en tant que Max, vous devez écrire ceci :

```bat
runas /user:Max "notepad.exe"
```

Une fois cette commande entrée, le mot de passe du profil Max vous sera demandé. Si vous voulez que le mot de passe de Max soit enregistré pour les autres lancements, vous devez écrire ceci :

```bat
runas /user:Max /savecreds "notepad.exe"
```

Une fois encore, le mot de passe du profil de Max vous sera demandé. Lors des lancements suivant avec la commande "runas", ce dernier ne vous sera plus demandé car il est sauvegardé dans votre session. Vous pouvez le vérifier en vous rendant dans la section "Informations d'identification Windows" du Gestionnaire d'identification.

La visibilité du mot de passe ainsi sauvegardé est un souci de sécurité : les personnes pouvant avoir accès à l'ordinateur peuvent récupérer ce mot de passe et ainsi accéder à la session d'un autre profil (ici Max); cet autre profil pouvant disposer de droits administrateur. C'est pour prévenir cela que l'application "Start As" a été créée.

En enregistrant le mot de passe dans un fichier (nommé "fichier d'authentification", avec comme extenstion crt), d'une façon chiffrée et sécurisée, "Start As" permet de partager ce fichier avec un autre utilisateur afin qu'il puisse lancer le programme cible souhaité, sans révéler le mot de passe. Dans les détails, il faudra d'abord créer un fichier d'authentification à l'aide de l'outil 'configSaveAs'. Ensuite, une fois ce fichier créé, il faudra le passer comme paramètre de l'exécutable "StartAsCmd.exe".

### Menace potentielle sur la sécurité

For security reasons, it is important that regular Windows users use a session with limited rights, as well as a session with administrator rights. Indeed, for a regularly use, a standard user does not need administrator rights and can ask for an elevation of privileges during installations. Linux, Android or iOs distributions work on the same principle. This precaution won't prevent or mitigate all malware infections. Some malware can "escalate" its system privileges and give itself powers that a limited user doesn't have. But regular, run-of-the-mill malware, which is what most people face most of the time, doesn't do that.

Read this article for more details : [Protect your computer with this one simple trick, TomsGuide.com, Paul Wagenseil, published March 20, 2019](https://www.tomsguide.com/us/limited-account-benefits,news-25682.html).

Based on this, "Start As" can make it easier to start applications as an administrator. Using a web browser can be risky in these conditions: viruses, malware or other security risks are more likely to corrupt the use of your computer. Preferably run browsers with a limited account.

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

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

1. Download latest release [here](https://github.com/knightofnet/StartAs/releases).
2. Extract the archive in a folder.

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

Two executables come with the downloaded release:
- ``StartAsCmd.exe``: this is the main executable. It is a bootstraper to start a target application using another profile; Other profile saved in an encrypted authentication file.
- ``ConfigStartAs.exe`` : this application allows the creation of authentication file.

### Create an authentification file

To start an application with another profile, it is necessary to create an authentication file first. This file will contain the connection information, as well as the target application, its working path, and possibly some launch arguments. Everything is encrypted using the SID of the computer and a unique Salt.

``Note: the unique salt is a constant in the . The commit version contains a default value, different from the one used for the releases.``

The creation of the authentication file is done by launching the executable 'ConfigStartAs.exe'. A window opens with different text fields that must be valued:

The path of the executable is the path to the file that must be launched with another profile (administrator or other). In the part "Start as", enter the user name and the password of the profile that will be used.

There are some security options that can be activated:

- The authentication file can have a limited validity in time. To do this, check the corresponding box and set an expiration date.
- It is also possible to perform an integrity test of the target executable file, at the time of launching with the profile of the other user. A SHA1 comparison will then be performed. This can slow down the launch of the application, but it guarantees that the executable is the right one (and not another one, with the same name in the same folder).
- Finally, it is possible to ask for a PIN code to start the target application. It is a code on 4 to 8 characters, only numbers. Only users with the PIN code will be able to start the target application, but still without knowing the password of the used profile.

Once everything is set up, you can click on the "Save" button in the lower part of the window. This will create the authentication file at the address specified in the "Authentication file" text box.

This file should be used with the 'StartAsCmd.exe' executable to start the target application.


<p align="right">(<a href="#top">back to top</a>)</p>


### Start application with authentification file

With the authentication file, use the executable as follows to start the target application with a different profile:

```
StartAsCmd.exe AuthFile.crt
```

It is also possible to start with arguments named :

```
StartAsCmd.exe -f AuthFile.crt [-w]
StartAsCmd.exe --authent-file AuthFile.crt [--wait]
```

Named arguments:
- ``-f`` : path to authentification file. Also ``--authent-file``.
- ``-w`` : by default, the target application is started with the saved profile without waiting for it to finish. With this setting, the end of the application is waited for.. Also ``--wait``.


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

Aryx - [@wolfaryx](https://twitter.com/wolfaryx) (wolfaryx [AT] gmail [DOT] com)

Project Link: [https://github.com/knightofnet/StartAs](https://github.com/knightofnet/StartAs)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* Template of this README.MD file available [here](https://github.com/othneildrew/Best-README-Template).

*I like to think that programming is like playing with legos: you assemble blocks to form algorithms, functions, classes. At the end, it gives a program! (... and then you just spend your time to make it even better, or you start from the beginning for another one!)*

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
[product-screenshot]: reposElements\configStartAs_MainView.png
