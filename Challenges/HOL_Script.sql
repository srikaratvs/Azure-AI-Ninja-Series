CREATE TABLE usertable(id int NOT NULL IDENTITY PRIMARY KEY, name varchar(100), Phone varchar(15), gender varchar(12), email varchar(100), faceid varchar(100));

CREATE TABLE imagevalidation(id int NOT NULL IDENTITY PRIMARY KEY, validation_type varchar(100), validation_message varchar(255),isactive int);

CREATE TABLE gesture(id int NOT NULL IDENTITY PRIMARY KEY, gesture_name varchar(100), thumbnail_url varchar(max), gesture_message varchar(255), isactive int);

CREATE TABLE auditlog(id int NOT NULL IDENTITY PRIMARY KEY, layer varchar(100), result_type varchar(50), device_type varchar(50), userimage text);

CREATE TABLE verifytime(id int NOT NULL IDENTITY PRIMARY KEY, personid varchar(100), date varchar(25), checkin varchar(25), checkout varchar(25));


INSERT INTO imagevalidation(validation_type, validation_message, isactive) VALUES('Face Availability','Face is not available',0);
INSERT INTO imagevalidation(validation_type, validation_message, isactive) VALUES('Remove Sun Glasses','Please remove sunglasses',0);
INSERT INTO imagevalidation(validation_type, validation_message, isactive) VALUES('Multiple Face','Multiple Faces are detected',0);
INSERT INTO imagevalidation(validation_type, validation_message, isactive) VALUES('Expressions','Expression is not Neutral',0);

INSERT INTO gesture(gesture_name, thumbnail_url, gesture_message, isactive) VALUES('One','https://aitechseries.blob.core.windows.net/gestureimage/1.JPEG','The Person is showing one',0);
INSERT INTO gesture(gesture_name, thumbnail_url, gesture_message, isactive) VALUES('Two','https://aitechseries.blob.core.windows.net/gestureimage/2.JPEG','The Person is showing Two',0);
INSERT INTO gesture(gesture_name, thumbnail_url, gesture_message, isactive) VALUES('Three','https://aitechseries.blob.core.windows.net/gestureimage/3.JPEG','The Person is showing Three',0);
INSERT INTO gesture(gesture_name, thumbnail_url, gesture_message, isactive) VALUES('Four','https://aitechseries.blob.core.windows.net/gestureimage/4.JPEG','The Person is showing Four',0);
INSERT INTO gesture(gesture_name, thumbnail_url, gesture_message, isactive) VALUES('Five','https://aitechseries.blob.core.windows.net/gestureimage/5.JPEG','The Person is showing Five',0);