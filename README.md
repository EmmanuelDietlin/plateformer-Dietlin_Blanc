## Platformer

Ce projet a été effectué dans le cadre du module d'IHM (Interactions Homme - Machine) dispensé en filière de 3ème année JIN (Jeux vidéos et Interactions Numériques) de Télécom Sudparis.

L'objectif a été de produire un jeu de plateforme, plus particulièrement chercher et intégrer des mécaniques de jeu de plateforme et les rendre agréables à utiliser pour le joueur.

Ce projet a été effectué avec Ludovic Blanc.

------

## Outils

Le jeu est développé avec le moteur de jeu Unity.

------

## Jeu

Le jeu se compose d'un seul niveau, dans lequel le joueur peut se déplacer pour tenter d'atteindre le point d'arrivée.

Le joueur peut effectuer plusieurs comportements :
- Déplacement horizontal
- Saut
- Dash sur le côté

Sont également disponibles plusieurs types de plateformes différentes :
- Sol orange, sur lequel le joueur se déplace normalement
- Sol vert, qui sont des plateformes que le joueur peut traverser
- Sol bleu, sur lequel le joueur peut rebondir
- Pics, qui donnent un dégât au joueur
- Sol blanc, qui sont des plateformes mouvantes

Au-delà de ces éléments, plusieurs mécaniques de jeu de plateformes sont implémentées :
- Coyote time, c'est-à-dire la possibilité d'effectuer un saut pendant quelques frames après avoir quitté une plateforme
- Double saut
- Déformation visuelle du personnage lors de l'atterissage ainsi que selon sa vitesse de déplacement horizontale
- Particules lors de l'atterissage
- Divers effets sonores
- Changement de couleur de personnage lorsque son double saut n'est plus disponible
- Effet visuel et sonore lorsque le joueur prend un dégât

Une grande partie des valeurs des variables régissant le comportement du personnage peuvent être ajustées par l'utilisateur

Le jeu peut être joué sur itch.io [ici](https://manudiet.itch.io/platformer).

