CREATE TABLE `language` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL
);

CREATE TABLE `language_level` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `language_id` integer NOT NULL,
  `name` varchar(50) NOT NULL
);

CREATE TABLE `course` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `language_level_id` integer NOT NULL,
  `name` varchar(255) NOT NULL,
  `info` text,
  `price` double
);

CREATE TABLE `client` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `surname` varchar(50) NOT NULL,
  `birthday` datetime NOT NULL,
  `phone` varchar(20),
  `email` varchar(100)
);

CREATE TABLE `teacher` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `surname` varchar(50) NOT NULL,
  `birthday` datetime NOT NULL,
  `phone` varchar(20),
  `email` varchar(100)
);

CREATE TABLE `client_language` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `client_id` integer NOT NULL,
  `language_level_id` integer NOT NULL,
  `last_experience` text,
  `needs` text
);

CREATE TABLE `teacher_language` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `teacher_id` integer NOT NULL,
  `language_level_id` integer NOT NULL
);

CREATE TABLE `group` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `teacher_id` integer NOT NULL,
  `course_id` integer NOT NULL,
  `name` varchar(50) NOT NULL
);

CREATE TABLE `client_in_group` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `group_id` integer NOT NULL,
  `client_id` integer NOT NULL
);

CREATE TABLE `payment` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `client_in_group_id` integer NOT NULL,
  `date` datetime NOT NULL,
  `count` double NOT NULL
);

CREATE TABLE `schedule` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `group_id` integer NOT NULL,
  `about` text,
  `datetime` datetime NOT NULL
);

CREATE TABLE `value` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL
);

CREATE TABLE `attendance_log` (
  `id` integer PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `value_id` integer,
  `schedule_id` integer NOT NULL,
  `client_in_group_id` integer NOT NULL
);

ALTER TABLE `language_level` ADD FOREIGN KEY (`language_id`) REFERENCES `language` (`id`);

ALTER TABLE `course` ADD FOREIGN KEY (`language_level_id`) REFERENCES `language_level` (`id`);

ALTER TABLE `client_language` ADD FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);

ALTER TABLE `client_language` ADD FOREIGN KEY (`language_level_id`) REFERENCES `language_level` (`id`);

ALTER TABLE `teacher_language` ADD FOREIGN KEY (`teacher_id`) REFERENCES `teacher` (`id`);

ALTER TABLE `teacher_language` ADD FOREIGN KEY (`language_level_id`) REFERENCES `language_level` (`id`);

ALTER TABLE `group` ADD FOREIGN KEY (`teacher_id`) REFERENCES `teacher` (`id`);

ALTER TABLE `group` ADD FOREIGN KEY (`course_id`) REFERENCES `course` (`id`);

ALTER TABLE `client_in_group` ADD FOREIGN KEY (`group_id`) REFERENCES `group` (`id`);

ALTER TABLE `client_in_group` ADD FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);

ALTER TABLE `payment` ADD FOREIGN KEY (`client_in_group_id`) REFERENCES `client_in_group` (`id`);

ALTER TABLE `schedule` ADD FOREIGN KEY (`group_id`) REFERENCES `group` (`id`);

ALTER TABLE `attendance_log` ADD FOREIGN KEY (`value_id`) REFERENCES `value` (`id`);

ALTER TABLE `attendance_log` ADD FOREIGN KEY (`schedule_id`) REFERENCES `schedule` (`id`);

ALTER TABLE `attendance_log` ADD FOREIGN KEY (`client_in_group_id`) REFERENCES `client_in_group` (`id`);
