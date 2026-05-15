CREATE DATABASE IF NOT EXISTS `igrashky` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `igrashky`;

CREATE TABLE IF NOT EXISTS `igrashky` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(30) NOT NULL COMMENT 'Назва іграшки',
  `price` float NOT NULL COMMENT 'Ціна',
  `amount` int(5) NOT NULL COMMENT 'Кількість',
  `ageRange` varchar(6) NOT NULL COMMENT 'Вікові межі',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1;

INSERT INTO `igrashky` (`name`, `price`, `amount`, `ageRange`) VALUES
('Конструктор LEGO', 1500.50, 25, '6-12'),
('М''яка іграшка Ведмідь', 450.00, 50, '3-5'),
('Настільна гра Монополія', 850.00, 15, '8-99'),
('Машинка радіокерована', 1200.00, 10, '6-14'),
('Лялька інтерактивна', 650.00, 30, '4-10');