CREATE TABLE `company_settings` (
   `settings_id` int NOT NULL ,
   `settings_name` varchar(45) DEFAULT NULL,
   `settings_value` varchar(255) DEFAULT NULL,
   PRIMARY KEY (`settings_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `currency` (
   `currency_id` int NOT NULL ,
   `currency_name` varchar(45) DEFAULT NULL,
   `currency_code` varchar(45) DEFAULT NULL,
   `currency_numeric_code` int DEFAULT NULL,
   `curreny_symbol` varchar(45) DEFAULT NULL,
   `currency_isDefault` tinyint(1) DEFAULT NULL,
   PRIMARY KEY (`currency_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=280 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `customer_objects` (
   `customer_id` int NOT NULL,
   `customer_company_id` varchar(45) DEFAULT '0',
   `customer_name` varchar(255) DEFAULT NULL,
   `customer_adress` varchar(255) DEFAULT NULL,
   `customer_postcode` varchar(255) DEFAULT NULL,
   `customer_city` varchar(255) DEFAULT NULL,
   `customer_country` varchar(255) DEFAULT NULL,
   `customer_website` varchar(255) DEFAULT NULL,
   `customer_phone` varchar(255) DEFAULT NULL,
   `customer_mail` varchar(255) DEFAULT NULL,
   `customer_contact` varchar(255) DEFAULT NULL,
   PRIMARY KEY (`customer_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `dbinfo` (
   `id` int NOT NULL,
   `dbinfo_key` varchar(45) DEFAULT NULL,
   `dbinfo_value` int DEFAULT NULL,
   PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `filter1_names` (
   `filter_id` int NOT NULL,
   `name` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`filter_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



CREATE TABLE `filter2_names` (
   `filter_id` int NOT NULL,
   `name` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`filter_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



CREATE TABLE `filter3_names` (
   `filter_id` int NOT NULL,
   `name` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`filter_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `filter4_names` (
   `filter_id` int NOT NULL,
   `name` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`filter_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `filter5_names` (
   `filter_id` int NOT NULL,
   `name` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`filter_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `floor_group_item_relations` (
   `id` int NOT NULL,
   `item_id` varchar(45) DEFAULT NULL,
   `group_id` varchar(45) DEFAULT NULL,
   `item_quantity` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `floor_group_objects` (
   `id` varchar(45) NOT NULL,
   `floor_id` varchar(45) DEFAULT NULL,
   `group_id` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `floor_objects` (
   `floor_id` int NOT NULL,
   `floor_name` varchar(45) DEFAULT NULL,
   `floor_quantity` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`floor_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `group_objects` (
   `group_id` int NOT NULL,
   `group_name` varchar(45) DEFAULT NULL,
   `group_quantity` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`group_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `history_log` (
   `history_id` int NOT NULL AUTO_INCREMENT,
   `item_id` int DEFAULT NULL,
   `item_quantity` int DEFAULT NULL,
   `item_location_old` int DEFAULT NULL,
   `item_location_new` int DEFAULT NULL,
   `old_zone` int DEFAULT '0',
   `new_zone` int DEFAULT '0',
   `action_id` int DEFAULT NULL,
   `user_id` int DEFAULT NULL,
   `createdAt` datetime DEFAULT NULL,
   `updatedAt` datetime DEFAULT NULL,
   `show_trigger` tinyint DEFAULT NULL,
   PRIMARY KEY (`history_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=127 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `item_filter_relations` (
   `item_id` int NOT NULL,
   `filter1_id` int DEFAULT NULL,
   `filter2_id` int DEFAULT NULL,
   `filter3_id` int DEFAULT NULL,
   `filter4_id` int DEFAULT NULL,
   `filter5_id` int DEFAULT NULL,
   PRIMARY KEY (`item_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `item_location_relations` (
   `id` int NOT NULL AUTO_INCREMENT,
   `item_id` int DEFAULT NULL,
   `location_id` int DEFAULT NULL,
   `location_item_quantity` int DEFAULT NULL,
   PRIMARY KEY (`id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=1013 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `item_objects` (
   `item_id` int NOT NULL,
   `item_ident` text,
   `item_quantity_total` int DEFAULT NULL,
   `item_quantity_total_new` int DEFAULT NULL,
   `item_onetime_use` int DEFAULT NULL,
   `item_quantity_min` int DEFAULT NULL,
   `item_orderquant_min` int DEFAULT '0',
   `item_orderable` tinyint(1) DEFAULT '0',
   `item_onorder` int DEFAULT '0',
   `item_description` text,
   `item_description_2` text,
   `item_image_path` text,
   PRIMARY KEY (`item_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `item_rents` (
   `rent_id` int NOT NULL AUTO_INCREMENT,
   `item_id` int DEFAULT NULL,
   `user_id` int DEFAULT NULL,
   `location_id` int DEFAULT NULL,
   `rent_quantity` int DEFAULT NULL,
   `machine_id` int DEFAULT NULL,
   `item_used` tinyint(1) DEFAULT '0',
   `createdAt` datetime DEFAULT NULL,
   PRIMARY KEY (`rent_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `item_subitem_relations` (
   `item_id` int NOT NULL,
   `subitem_id` int NOT NULL,
   PRIMARY KEY (`item_id`,`subitem_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `item_vendor_relations` (
   `item_id` int NOT NULL,
   `vendor_id` int DEFAULT NULL,
   `item_price` varchar(45) DEFAULT NULL,
   `currency_id` int DEFAULT NULL,
   PRIMARY KEY (`item_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `location_objects` (
   `location_id` int NOT NULL AUTO_INCREMENT,
   `location_name` varchar(45) DEFAULT NULL,
   `location_size` varchar(45) DEFAULT NULL,
   `location_quantity` varchar(45) DEFAULT NULL,
   `item_used` tinyint DEFAULT NULL,
   `item_constructed` tinyint DEFAULT NULL,
   PRIMARY KEY (`location_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=2365 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `log_action_names` (
   `action_id` int NOT NULL,
   `name_DE` varchar(45) DEFAULT NULL,
   `name_EN` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`action_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `machines` (
   `machine_id` int NOT NULL,
   `name` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`machine_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `order_item_relations` (
   `order_id` int NOT NULL,
   `order_ident` varchar(255) DEFAULT NULL,
   `item_id` int DEFAULT NULL,
   `order_quantity` int DEFAULT NULL,
   `order_quantity_org` int DEFAULT NULL,
   `vendor_id` int DEFAULT NULL,
   `isOpen` tinyint DEFAULT NULL,
   `createdAt` datetime DEFAULT NULL,
   PRIMARY KEY (`order_id`),
   KEY `order_id_idx` (`order_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `order_objects` (
   `order_id` int NOT NULL,
   `order_ident` varchar(45) DEFAULT NULL,
   `order_status` tinyint DEFAULT NULL,
   PRIMARY KEY (`order_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

 CREATE TABLE `culture_objects` (
   `culture_id` int NOT NULL,
   `culture_name` varchar(255) DEFAULT NULL,
   PRIMARY KEY (`culture_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

 CREATE TABLE `settings` (
   `settings_id` int NOT NULL,
   `settings_name` varchar(45) DEFAULT NULL,
   `settings_input` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`settings_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `syncdatatable` (
   `id` int NOT NULL,
   `item_ident` varchar(45) DEFAULT NULL,
   `item_location` varchar(45) DEFAULT NULL,
   `item_location_quantity` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `user_privilege_relations` (
   `id` int NOT NULL,
   `user_id` int DEFAULT NULL,
   `privilege_id` int DEFAULT NULL,
   PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ;



CREATE TABLE `user_privileges` (
   `privileges_id` int NOT NULL ,
   `privileges_name` varchar(45) DEFAULT NULL,
   `privileges_name_DE` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`privileges_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

 

CREATE TABLE `user_roles` (
   `role_id` int NOT NULL ,
   `name` varchar(45) DEFAULT NULL,
   PRIMARY KEY (`role_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



CREATE TABLE `users` (
   `user_id` int NOT NULL ,
   `user_ident` varchar(45) DEFAULT NULL,
   `username` varchar(45) DEFAULT NULL,
   `user_password` varchar(255) DEFAULT NULL,
   `name` varchar(45) DEFAULT NULL,
   `surname` varchar(45) DEFAULT NULL,
   `email` varchar(45) DEFAULT NULL,
   `role_id` int DEFAULT NULL,
   `culture_id` int NOT NULL,
   PRIMARY KEY (`user_id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



 CREATE TABLE `vendor_objects` (
   `vendor_id` int NOT NULL,
   `vendor_name` varchar(45) DEFAULT NULL,
   `vendor_adress` varchar(45) DEFAULT NULL,
   `vendor_postcode` varchar(45) DEFAULT NULL,
   `vendor_city` varchar(45) DEFAULT NULL,
   `vendor_country` varchar(45) DEFAULT NULL,
   `vendor_website` varchar(45) DEFAULT NULL,
   `vendor_phone` varchar(45) DEFAULT NULL,
   `vendor_mail` varchar(45) DEFAULT NULL,
   `vendor_contact` varchar(45) DEFAULT NULL,
   `vendor_auto_order` tinyint(1) DEFAULT '0',
   PRIMARY KEY (`vendor_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;