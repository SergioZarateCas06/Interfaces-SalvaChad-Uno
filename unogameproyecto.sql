-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: unogameproyecto
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `movimientos`
--

DROP TABLE IF EXISTS `movimientos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimientos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `partida_id` int NOT NULL,
  `numero_turno` int NOT NULL,
  `nombre_jugador` varchar(100) NOT NULL,
  `accion` varchar(50) NOT NULL,
  `descripcion_carta` varchar(50) DEFAULT NULL,
  `fecha` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movimientos`
--

LOCK TABLES `movimientos` WRITE;
/*!40000 ALTER TABLE `movimientos` DISABLE KEYS */;
INSERT INTO `movimientos` VALUES (1,1,1,'Edson','Jugar','Rojo Salta','2026-10-11 18:53:15'),(2,1,2,'Erik','Jugar','Amarillo Salta','2026-10-11 18:53:17'),(3,1,3,'Edson','Jugar','Comodin','2026-10-11 18:53:23'),(4,1,4,'Sergio','Jugar','Azul 2','2026-10-11 18:53:24'),(5,1,5,'Erik','Jugar','Azul 3','2026-10-11 18:53:25'),(6,1,6,'Enrique','Jugar','Azul Salta','2026-10-11 18:53:26'),(7,1,7,'Sergio','Jugar','Azul 6','2026-10-11 18:53:29'),(8,1,8,'Erik','Jugar','Comodin +4','2026-10-11 18:53:33'),(9,1,9,'Edson','Jugar','Azul 0','2026-10-11 18:53:37'),(10,1,10,'Sergio','Jugar','Azul 6','2026-10-11 18:53:38'),(11,1,11,'Erik','Jugar','Azul 9','2026-10-11 18:53:40'),(12,1,12,'Enrique','Jugar','Azul 7','2026-10-11 18:53:42'),(13,1,13,'Edson','Jugar','Azul Reversa','2026-10-11 18:53:44'),(14,1,14,'Enrique','Jugar','Azul 4','2026-10-11 18:53:46'),(15,1,15,'Erik','Jugar','Comodin','2026-10-11 18:53:48'),(16,1,16,'Sergio','Jugar','Amarillo 7','2026-10-11 18:53:49'),(17,1,17,'Edson','Robar','Verde 5','2026-10-11 18:53:52'),(18,1,18,'Enrique','Robar','Rojo 4','2026-10-11 18:53:54'),(19,1,19,'Erik','Jugar','Amarillo 1','2026-10-11 18:53:57'),(20,1,20,'Sergio','Jugar','Amarillo 9','2026-10-11 18:54:00'),(21,1,21,'Edson','Jugar','Comodin +4','2026-10-11 18:54:02'),(22,1,22,'Erik','Jugar','Comodin','2026-10-11 18:54:09'),(23,2,1,'1','Jugar','Rojo 3','2026-10-11 21:50:31'),(24,3,1,'Edson','Jugar','Azul 4','2026-10-12 17:23:46'),(25,3,2,'Erik','Jugar','Azul 4','2026-10-12 17:23:47'),(26,3,3,'Sergio','Jugar','Rojo 4','2026-10-12 17:24:00'),(27,3,4,'Enrique','Jugar','Rojo 2','2026-10-12 17:24:01'),(28,3,5,'Edson','Jugar','Rojo 6','2026-10-12 17:24:03'),(29,3,6,'Erik','Jugar','Verde 6','2026-10-12 17:24:04'),(30,3,7,'Sergio','Jugar','Amarillo 6','2026-10-12 17:24:05'),(31,3,8,'Enrique','Jugar','Amarillo Reversa','2026-10-12 17:24:06'),(32,3,9,'Sergio','Jugar','Amarillo 6','2026-10-12 17:24:07'),(33,3,10,'Erik','Jugar','Amarillo 4','2026-10-12 17:24:08'),(34,3,11,'Edson','Jugar','Amarillo 7','2026-10-12 17:24:10'),(35,3,12,'Enrique','Robar','Verde 9','2026-10-12 17:24:12'),(36,3,13,'Sergio','Jugar','Amarillo 2','2026-10-12 17:24:13'),(37,3,14,'Erik','Jugar','Verde 2','2026-10-12 17:24:15'),(38,3,15,'Edson','Jugar','Verde 7','2026-10-12 17:24:16'),(39,3,16,'Enrique','Jugar','Verde 8','2026-10-12 17:24:17'),(40,3,17,'Sergio','Jugar','Verde Salta','2026-10-12 17:24:18'),(41,3,18,'Edson','Jugar','Verde 3','2026-10-12 17:24:20'),(42,3,19,'Enrique','Jugar','Comodin','2026-10-12 17:24:22'),(43,3,20,'Sergio','Jugar','Amarillo 7','2026-10-12 17:24:23'),(44,3,21,'Erik','Jugar','Azul 7','2026-10-12 17:24:25'),(45,3,22,'Edson','Jugar','Azul 2','2026-10-12 17:24:26'),(46,3,23,'Enrique','Jugar','Azul 1','2026-10-12 17:24:28'),(47,3,24,'Sergio','Jugar','Azul 7','2026-10-12 17:24:30'),(48,3,25,'Erik','Jugar','Azul 9','2026-10-12 17:24:32'),(49,3,26,'Edson','Robar','Rojo 8','2026-10-12 17:24:33'),(50,3,27,'Enrique','Jugar','Verde 9','2026-10-12 17:24:35'),(51,3,28,'Sergio','Jugar','Verde Salta','2026-10-12 17:24:36'),(52,3,29,'Edson','Robar','Azul 8','2026-10-12 17:24:38'),(53,3,30,'Enrique','Jugar','Verde 4','2026-10-12 17:24:40'),(54,3,31,'Sergio','Jugar','Verde +2','2026-10-12 17:24:42'),(55,3,32,'Edson','Robar','Azul 6','2026-10-12 17:24:45'),(56,3,33,'Enrique','Jugar','Verde 2','2026-10-12 17:24:46'),(57,4,1,'Edson','Jugar','Verde 0','2026-10-13 10:12:39'),(58,4,2,'Erik','Jugar','Verde 7','2026-10-13 10:12:41'),(59,4,3,'Enrique','Jugar','Verde +2','2026-10-13 10:12:42'),(60,4,4,'Edson','Jugar','Comodin +4','2026-10-13 10:12:47'),(61,4,5,'Enrique','Jugar','Rojo Reversa','2026-10-13 10:12:51'),(62,4,6,'Erik','Jugar','Rojo 4','2026-10-13 10:12:53'),(63,4,7,'Edson','Jugar','Rojo 5','2026-10-13 10:12:54'),(64,4,8,'Sergio','Jugar','Rojo 8','2026-10-13 10:12:56'),(65,4,9,'Enrique','Jugar','Rojo Reversa','2026-10-13 10:12:58'),(66,4,10,'Sergio','Jugar','Rojo 3','2026-10-13 10:12:59'),(67,4,11,'Edson','Jugar','Rojo Salta','2026-10-13 10:13:01'),(68,4,12,'Enrique','Jugar','Azul Salta','2026-10-13 10:13:04'),(69,4,13,'Edson','Robar','Rojo +2','2026-10-13 10:13:06'),(70,4,14,'Erik','Jugar','Comodin','2026-10-13 10:13:09'),(71,4,15,'Enrique','Robar','Azul 7','2026-10-13 10:13:12'),(72,4,16,'Sergio','Robar','Verde 3','2026-10-13 10:13:21'),(73,4,17,'Edson','Jugar','Rojo +2','2026-10-13 10:13:23'),(74,4,18,'Enrique','Robar','Azul Reversa','2026-10-13 10:13:27'),(75,4,19,'Sergio','Robar','Amarillo 3','2026-10-13 10:13:31'),(76,4,20,'Edson','Robar','Amarillo 2','2026-10-13 10:13:33'),(77,4,21,'Erik','Jugar','Rojo 3','2026-10-13 10:13:35'),(78,4,22,'Enrique','Robar','Azul 5','2026-10-13 10:13:38'),(79,4,23,'Sergio','Jugar','Verde 3','2026-10-13 10:13:40'),(80,4,24,'Edson','Jugar','Verde 5','2026-10-13 10:13:41'),(81,4,25,'Erik','Robar','Amarillo 9','2026-10-13 10:13:43'),(82,4,26,'Enrique','Robar','Rojo Salta','2026-10-13 10:13:45'),(83,4,27,'Sergio','Robar','Amarillo +2','2026-10-13 10:13:46'),(84,4,28,'Edson','Jugar','Verde 4','2026-10-13 10:13:47'),(85,4,29,'Erik','Jugar','Comodin','2026-10-13 10:13:52'),(86,4,30,'Enrique','Jugar','Amarillo 1','2026-10-13 10:13:53'),(87,4,31,'Sergio','Jugar','Azul 1','2026-10-13 10:13:55'),(88,4,32,'Edson','Robar','Amarillo 5','2026-10-13 10:13:59'),(89,4,33,'Erik','Jugar','Comodin +4','2026-10-13 10:14:01'),(90,4,34,'Sergio','Jugar','Azul 2','2026-10-13 10:14:06'),(91,4,35,'Edson','Jugar','Amarillo 2','2026-10-13 10:14:09'),(92,4,36,'Erik','Jugar','Amarillo 6','2026-10-13 10:14:13'),(93,4,37,'Enrique','Jugar','Verde 6','2026-10-13 10:14:16'),(94,4,38,'Sergio','Jugar','Amarillo 6','2026-10-13 10:14:19'),(95,4,39,'Edson','Jugar','Amarillo 5','2026-10-13 10:14:24'),(96,4,40,'Erik','Jugar','Amarillo 8','2026-10-13 10:14:27'),(97,4,41,'Enrique','Jugar','Amarillo +2','2026-10-13 10:14:30'),(98,4,42,'Edson','Jugar','Amarillo 0','2026-10-13 10:14:33'),(99,5,1,'1','Jugar','Amarillo 5','2026-10-13 10:20:48'),(100,5,2,'2','Jugar','Amarillo 7','2026-10-13 10:21:18');
/*!40000 ALTER TABLE `movimientos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `participantespartida`
--

DROP TABLE IF EXISTS `participantespartida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `participantespartida` (
  `id` int NOT NULL AUTO_INCREMENT,
  `partida_id` int NOT NULL,
  `nombre_jugador` varchar(100) NOT NULL,
  `gano` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `partida_id` (`partida_id`),
  CONSTRAINT `participantespartida_ibfk_1` FOREIGN KEY (`partida_id`) REFERENCES `partidas` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `participantespartida`
--

LOCK TABLES `participantespartida` WRITE;
/*!40000 ALTER TABLE `participantespartida` DISABLE KEYS */;
INSERT INTO `participantespartida` VALUES (1,1,'Edson',0),(2,1,'Sergio',0),(3,1,'Erik',1),(4,1,'Enrique',0),(5,2,'1',0),(6,2,'2',0),(7,2,'3',0),(8,2,'4',0),(9,3,'Edson',0),(10,3,'Erik',0),(11,3,'Sergio',0),(12,3,'Enrique',1),(13,4,'Edson',1),(14,4,'Erik',0),(15,4,'Enrique',0),(16,4,'Sergio',0),(17,5,'1',0),(18,5,'2',0),(19,5,'3',0),(20,5,'4',0);
/*!40000 ALTER TABLE `participantespartida` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `partidas`
--

DROP TABLE IF EXISTS `partidas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `partidas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `partidas`
--

LOCK TABLES `partidas` WRITE;
/*!40000 ALTER TABLE `partidas` DISABLE KEYS */;
INSERT INTO `partidas` VALUES (1,'2026-10-11 18:53:00'),(2,'2026-10-11 21:50:21'),(3,'2026-10-12 17:23:35'),(4,'2026-10-13 10:12:26'),(5,'2026-10-13 10:20:25');
/*!40000 ALTER TABLE `partidas` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-15 10:32:05
