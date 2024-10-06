# Projet 6 : Vuforia avec API maison et base de données
 Voici le projet 6 de la formation Unity XR de MakeYourGame. Le but de ce projet est de reprendre l'application créée dans le cadre du projet 5, et d'y ajouter une couche d'appels en API conçus à la main, et hébergés sur un serveur local MAMP.

# Téléchargement
Pour télécharger l'APK de l'application, veuillez cliquer [ici](https://drive.google.com/file/d/14FTTjKXGNXAHL43ngOuITiRbeW1un7Ta/view?usp=sharing).

# Installation du projet en local
## Fichier trop volumineux
Si vous téléchargez le projet Unity en lui-même, vous risquez de rencontrer une erreur, causée par le manque d'un fichier qui est bien trop large pour pouvoir être inclus dans le repository Github. Le fichier en question peut être téléchargé avec [ce lien ci](https://drive.google.com/file/d/1hmGrSk40-OTc45cAcZSkU2HGcU2mP-4a/view?usp=drive_link), et doit être placé dans le dossier Packages/.

## Installation serveur local
Pour que l'application fonctionne correctement, un serveur local doit être hébergé. Pour se faire, une application de type MAMP ou XAMP doit être utilisée. Voici comment installer le serveur avec MAMP :
- Téléchargez et installez MAMP [ici](https://www.mamp.info/en/windows/).
- Accédez au dossier MAMP dans votre disque local C (ou dans l'emplacement d'installation de votre choix).
- Allez dans le dossier htdocs.
- Extrayez ce dossier compressé [ici](https://drive.google.com/file/d/13o4lANSJdffc-EaUeLSjUC51a7JsmN91/view?usp=sharing).
- Démarrez MAMP, et activez le serveur.

Une fois cela fait, il ne vous reste qu'à créer la base de données qui va stocker les données de l'application.

Pour cela, dans l'application MAMP, cliquez sur "Open WebStart Page", puis sur la fenêtre qui s'affiche, allez dans "TOOLS" en haut de page, puis "PHPMYADMIN". Ensuite, sur cette nouvelle page, créez une nouvelle base de données nommée "ikear". Après cela, il vous suffit de cliquer sur l'ongler "Import", et d'y insérer [ce fichier là](https://drive.google.com/file/d/1JGGFO8X0DzNgdlEZuzJLGrq6ae6lbp0V/view?usp=sharing). Lancez le script, et voilà, la base de données s'est générée.

À noter que par défaut, un utilisateur administrateur existe, avec l'identifiant "admin" et le mot de passe "admin".

# Interface
## Accueil
 L'utilisateur, lorsqu'il lance l'application, est accueilli par la page d'accueil de l'application.
 
<img src="https://github.com/user-attachments/assets/4b25964e-2522-4ab8-bc27-5f6e73d56cb7" width=270 height=600 margin=auto>

À partir d'ici, il peut accéder au magasin en cliquant sur le premier bouton, au test de mobilier dans la Réalité Augmentée (RA) avec le deuxième bouton. Ensuite, avec les boutons de pied de page commun à toutes les pages, il peut retourner en arrière (si son historique de navigation est vide, il quitte l'application en appuyant dessus), retourner à l'accueil s'il n'y est pas déjà, et accéder à un menu de navigation rapide, qui ressemble à ceci :

<img src=https://github.com/user-attachments/assets/8a37babe-dc89-4f02-b864-a4137327e278 width=270 height=600 margin=auto>

## Magasin
En accédant le magasin, l'utilisateur peut voir la liste des meubles disponibles dans le magasin.

<img src=https://github.com/user-attachments/assets/6dee529e-6752-411d-bb9b-f3b347ac1349 width=270 height=600 margin=auto>

Sur cette page, pour chaque meuble affiché, on peut voir une image de prévisualisation du meuble, son nom, une description, et son prix, tout ça sur un bouton.
En cliquant sur l'un de ces boutons, l'utilisateur arrive sur la page de détails du meuble sélectionné.

## Détails de meuble

<img src=https://github.com/user-attachments/assets/efaa3faf-11be-453e-a220-41f9d5b72a56 width=270 height=600 margin=auto>

Une fois sur cette page, l'utilisateur peut voir une version mieux définie de la prévisualisation du meuble, ses dimensions, sa description complète, et en pied de page, le prix, avec l'option d'ajouter le meuble au panier, ou de le tester avec le mode de test RA.

## Panier

<img src=https://github.com/user-attachments/assets/752faab7-0124-4bef-a78b-a09ac153c88f width=270 height=600 margin=auto>

Après avoir ajouter un objet au panier, ou en accédant la page par le biais de la navigation rapide, l'utilisateur peut accéder au panier. Depuis cette page, il peut incrémenter, décrémenter, ou même retirer entièrement un meuble du panier. Avec chaque interaction, le total du panier est mis à jour en bas à droite.

# Test de Réalité Augmentée

En entrant le mode de test de meubles en RA, l'utilisateur peut appuyer sur les boutons en bas, correspondant au meuble à insérer, afin de les ajouter à la scène.

<img src=https://github.com/user-attachments/assets/c0b2c909-c725-43a3-9779-fc1693681aa0 width=270 height=600 margin=auto>

## Mode mouvement
Une fois un meuble placé, l'utilisateur peut déplacer le meuble en cliquant sur le bouton de déplacement (les 4 flèches en croix), puis faire glisser le doigt sur l'écran pour le faire déplacer.

<img src=https://github.com/user-attachments/assets/4f484446-e1f6-435e-a07d-7c765563f8b3 width=270 height=600 margin=auto>

## Mode rotation
L'utilisateur peut aussi faire tourner un meuble sur lui-même en appuyant sur le bouton de rotation de l'objet à faire tourner (les 2 flèches en demi-cercle).

<img src=https://github.com/user-attachments/assets/5da471f3-239f-4689-a422-db2fce2fc072 width=270 height=600 margin=auto>

## Suppression de meuble
L'utilisateur a aussi l'option de retirer un meuble de la scène en appuyant sur le bouton de suppression (la corbeille).

<img src=https://github.com/user-attachments/assets/1e7607fb-3593-4be2-b5f8-89a83b475b4a width=270 height=600 margin=auto>

## Passage du mode RA vers le panier
Enfin, l'utilisateur peut prendre tous les objets placés dans la scène, et les ajoute dans leur panier afin d'être acheté plus tard.

<img src=https://github.com/user-attachments/assets/055d4d8e-0d1c-4515-9140-916ea939cf19 width=270 height=600 margin=auto>

# Note
Si vous téléchargez le projet Unity en lui-même, vous risquez de rencontrer une erreur, causée par le manque d'un fichier qui est bien trop large pour pouvoir être inclus dans le repository Github. Le fichier en question peut être téléchargé avec [ce lien ci](https://drive.google.com/file/d/1hmGrSk40-OTc45cAcZSkU2HGcU2mP-4a/view?usp=drive_link), et doit être placé dans le dossier Packages/.
