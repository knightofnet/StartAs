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

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>

### Built With

* [AryxDevLibrary (by me)](https://www.nuget.org/packages/AryxDevLibrary/)

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>

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

Pour des raisons de sécurité, il est important que les utilisateurs réguliers de Windows utilisent une session avec des droits limités, ainsi qu'une session avec des droits d'administrateur. En effet, pour une utilisation régulière, un utilisateur standard n'a pas besoin des droits d'administrateur et peut demander une élévation de privilèges lors des installations (ou lorsque cela est nécessaire). Les distributions Linux, Android ou iOs fonctionnent sur le même principe. Cette précaution ne permettra pas de prévenir ou d'atténuer toutes les infections par des logiciels malveillants, en effet certains logiciels malveillants peuvent "élever" leurs privilèges système et se donner des pouvoirs qu'un utilisateur limité n'a pas. Mais les logiciels malveillants ordinaires, qui sont ceux auxquels la plupart des gens sont confrontés la plupart du temps, ne font pas cela.

Lisez cet article pour plus de détails : [Protect your computer with this one simple trick, TomsGuide.com, Paul Wagenseil, publié le 20 mars 2019](https://www.tomsguide.com/us/limited-account-benefits,news-25682.html).

Sur cette base, "StartAs" peut faciliter le lancement d'applications en tant qu'administrateur. 

!! Attention !! : l'utilisation d'un navigateur web peut être risquée dans ces conditions : les virus, malwares ou autres risques de sécurité sont plus susceptibles de corrompre l'utilisation de votre ordinateur. Exécutez de préférence les navigateurs avec un compte limité.

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>

<!-- GETTING STARTED -->
## Avant-propos

Pour installer "StartAs" et le démarrer, suivez ces quelques étapes.

### Pré-requis

Cette application fonctionne sous Microsoft Windows avec le .net Framework 4.8.

Pour tester que vous avez la version minimum requise, vous pouvez utiliser cette commande Powershell :

1. Ouvrez Powershell en écrivant ```powershell``` dans une invite de commandes, ou dans le menu démarrer.
2. Ecrivez le texte suivant (ou copiez-collez le). Validez avec la touche Entrée du clavier :

``` powershell
(Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full").Release -ge 528040
```

3. Si vous voyez ```True```, alors tout est OK. Sinon, téléchargez et installez .net Framework 4.8  en vous rendez sur ce site: [télécharger .Net Framework 4.8 Runtime (web-installer)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer).

### Installation

1. Télécharger la dernière verison de StartAs [ici](https://github.com/knightofnet/StartAs/releases).
2. Décompresez l'archive téléchargés dans le dossier de votre choix.

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>



<!-- USAGE EXAMPLES -->
## Utilisation

Plusieurs fichiers exécutables sont fournit avec la release:
- ``StartAsCmd.exe`` : C'est l'exécutable principal. Il agit comme un lanceur de l'applicaiton cible, qu'il faut démarrer avec un autre profil; Les informations de connexion à cet autre profil sont sauvegardées dans un fichier d'authentification chiffré.
- ``ConfigStartAs.exe`` : Cette application va vous permettre la création de fichier d'authentification.
- ``StartAsNoWin.exe`` : Il s'agit d'un équivalent de ``StartAsCmd.exe`` mais ce dernier se lance sans afficher aucune fenêtre de chargement.

### Créer un fichier d'authentification

Pour démarrer une application avec un autre profil, il est d'abord nécessaire de créer un fichier d'authentification. Ce dernier contiendra les informaitons de connexion (comme le nom d'utilisateur et le mot de passe), ainsi que le chemin de l'applicaiton cible, son dossier de travail et si besoin les arguments de lancement. Tout est chiffré en se basant sur le SID de l'ordinateur et un sel unique.

``Note: le sel est unique et une constante. Le fichier présent dans le repo GitHub possède une valeur par défaut, différente de celle utilisée par l'application fournit.``

La création du fichier d'authentification est réalisée à l'aide l'exécutable 'ConfigStartAs.exe'. Dès qu'elle est lancée, une fenêtre s'ouvre avec différents champs textuels qu'il faut compléter afin de permettre la création du fichier d'authentification :

Le chemin du fichier exécutable cible est le chemin vers le fichier qu'il va falloir démarrera vec un autre profil (administrateur ou un autre). Dans la partie "Démarrer en tant que", entrez le nom d'utilisateur ainsi que le mot de passe associé.

Quelques options de sécurité supplémentaires sont présentes :

- Le fichier d'authentificaiton peut avoir une date de validité limitée : une date d'expiration. Pour utiliser cela, cochez la case et choisissez la date d'expiration.
- Il est possible de réaliser un test d'intégrité du fichier cible au moment du lancement avec le profil d'un autre utilisateur. Une comparaison de l'empreinte SHA1 sera alors réalisée. Cela peut ralentir le lancement de l'application cible, mais cela permet de garantir que l'exécutable cible est bien le bon à lancer (et non un fichier qui aurait le même nom, dans le même dossier).
- Enfin, il est également possible de demander un code PIN pour le démarrage de l'application cible. Il s'agit d'un code de 4 à 8 caractères composé uniquement de chiffres. Seuls les utilisateur davec le code PIN pourront démarrer l'application cible; il s'agit bien sur d'utiliser un code différent de mot de passse de l'utilisateur cible

Quand vous avez terminer de tout paramétrer, cliquez sur le bouton "Enregistrer" dans la partie inférieure de la fenêtre. Cela va créer le fichier d'authentification à l'adresse spécifiée dans le champs textuel "Fichier d'authentification".

Ce fichier créé, pourra être utilisé avec les exécutablee 'StartAsCmd.exe' ou ''StartAsNoWin.exe' afin de démarrer l'application cible.


<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>


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


<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>


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

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>



<!-- CONTACT -->
## Contact

Aryx - [@wolfaryx](https://twitter.com/wolfaryx) (wolfaryx [AT] gmail [DOT] com)

Project Link: [https://github.com/knightofnet/StartAs](https://github.com/knightofnet/StartAs)

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* Template of this README.MD file available [here](https://github.com/othneildrew/Best-README-Template).

*I like to think that programming is like playing with legos: you assemble blocks to form algorithms, functions, classes. At the end, it gives a program! (... and then you just spend your time to make it even better, or you start from the beginning for another one!)*

<p align="right">(<a href="#top">Retourner au début de la page</a>)</p>



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
