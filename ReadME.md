# Documentation de l'API C# - Baptiste Soulier, Djebril Harhad, Mathis Paillard

Bienvenue dans la documentation de l'API C# conçue pour gérer les opérations CRUD (Create, Read, Update, Delete) sur les utilisateurs (User) et les articles. Cette API est développée en utilisant C# et est destinée à être utilisée avec un serveur web sur Docker.

## 1. Opérations CRUD

L'API propose des fonctionnalités complètes pour les opérations CRUD, permettant la gestion efficace des utilisateurs (User) et des articles. Ces opérations comprennent la création, la lecture, la mise à jour et la suppression de données.

## 2. Serveur Web Docker

L'API est conçue pour fonctionner avec un serveur web sous Docker.

### Qu'est-ce que Docker ?

Docker est une plateforme de conteneur qui permet de créer facilement des conteneurs et des applications basées sur les conteneurs. Ces conteneurs sont des packages ou des applications isolées de toutes leurs dépendances, ce qui permet un déploiement et une gestion plus rapide, plus simple, et évite les conflits potentiels entre certaines applications.

### Pourquoi Docker ?

- **Isolation :** Isolation des applications sous forme de conteneurs pour éviter des conflits entre applications.

- **Portabilité :** Les conteneurs sont portables et peuvent être déployés de manière cohérente sur différentes machines, facilitant ainsi la configuration et les mises à jour.

- **Communication :** La communication entre les serveurs web et les serveurs de bases de données est simplifiée grâce à la mise en réseau Docker, facilitant ainsi l'échange d'informations entre les conteneurs.

### Étapes de Configuration :

1. **Installation de Docker :**
   - Suivez les instructions du site officiel pour installer Docker.

2. **Construction de l'image Docker :**
   - Utilisez le Dockerfile fourni.
     docker build -t nom_de_votre_image .

3. **Exécution du Conteneur Docker (Serveur Web) :**
   - Spécifiez le port lors de l'exécution.

     docker run -p 8080:80 nom_de_votre_image

4. **Accès à l'API :**
   - Une fois le serveur en cours d'exécution, l'API sera accessible via l'adresse [http://localhost:8080/api](http://localhost:8080/api).

# Organisation sur Trello

   - https://github.com/BaptSoulier/E-commerce-C-.git