USE ClassroomDeviceManagement;

-- ==========================================================================================================
-- = Thêm dữ liệu cho LOOKUP TABLES																	=
-- ==========================================================================================================
-- Insert user roles
INSERT INTO user_role (name) VALUES 
('Admin'), ('Teacher'), ('Student');

-- Insert device categories
INSERT INTO device_category (name) VALUES 
('Laptop'), ('Projector'), ('USB'), ('Microphone'), ('Speaker'), ('Camera');

-- Insert status values
INSERT INTO instance_status (name) VALUES 
('Available'), ('Borrowed'), ('Broken'), ('Maintenance');

INSERT INTO request_status (name) VALUES 
('Pending'), ('Approved'), ('Rejected'), ('Returned');

INSERT INTO report_status (name) VALUES 
('New'), ('InProgress'), ('Resolved');


-- ==========================================================================================================
-- = Thêm một số models cho mỗi category																	=
-- ==========================================================================================================

-- Laptop (category_id = 1)
INSERT INTO device_model (model_name, category_id, specifications, storage_location) 
VALUES 
('Dell XPS 13', 1, 'Intel i7, 16GB RAM, 512GB SSD', 'A1.101'),
('MacBook Pro', 1, 'Apple M1, 16GB RAM, 512GB SSD', 'A1.101'),
('Lenovo ThinkPad', 1, 'Intel i5, 8GB RAM, 256GB SSD', 'A1.101');

-- Projector (category_id = 2)
INSERT INTO device_model (model_name, category_id, specifications, storage_location) VALUES
('Epson EB-X41', 2, '3600 Lumens, XGA', 'A1.101'),
('BenQ MW535', 2, '3600 Lumens, WXGA', 'A1.101');

-- USB (category_id = 3)
INSERT INTO device_model (model_name, category_id, specifications, storage_location) VALUES
('Kingston 32GB', 3, 'USB 3.0', 'A1.101'),
('SanDisk 64GB', 3, 'USB 3.1', 'A1.101');

-- Microphone (category_id = 4)
INSERT INTO device_model (model_name, category_id, specifications, storage_location) VALUES
('Shure SM58', 4, 'Dynamic, Cardioid', 'A1.101'),
('Blue Yeti', 4, 'USB Condenser', 'A1.101');

-- Speaker (category_id = 5)
INSERT INTO device_model (model_name, category_id, specifications, storage_location) VALUES
('Bose Companion 2', 5, '2.0 Channel', 'A1.101'),
('JBL Professional', 5, 'PA Speaker', 'A1.101');

-- Camera (category_id = 6)
INSERT INTO device_model (model_name, category_id, specifications, storage_location) VALUES
('Canon EOS 200D', 6, 'DSLR, 24.2MP', 'A1.101'),
('Sony Alpha 7C', 6, 'Mirrorless, 24MP', 'A1.101');

-- ==========================================================================================================
-- = Thêm một số models cho mỗi category																	=
-- ==========================================================================================================

-- Laptop Instances
INSERT INTO device_instance (instance_code, model_id, status_id, current_location) VALUES
('LAP-001', 1, 1, 'A1.101'),
('LAP-002', 1, 1, 'A1.101'),
('LAP-003', 2, 1, 'A1.101'),
('LAP-004', 2, 1, 'A1.101'),
('LAP-005', 3, 1, 'A1.101');

-- Projector Instances
INSERT INTO device_instance (instance_code, model_id, status_id, current_location) VALUES
('PRJ-001', 4, 1, 'A1.101'),
('PRJ-002', 5, 1, 'A1.101');

-- USB Instances
INSERT INTO device_instance (instance_code, model_id, status_id, current_location) VALUES
('USB-001', 6, 1, 'A1.101'),
('USB-002', 7, 1, 'A1.101');

-- Microphone Instances
INSERT INTO device_instance (instance_code, model_id, status_id, current_location) VALUES
('MIC-001', 8, 1, 'A1.101'),
('MIC-002', 9, 1, 'A1.101');

-- Speaker Instances
INSERT INTO device_instance (instance_code, model_id, status_id, current_location) VALUES
('SPK-001', 10, 1, 'A1.101'),
('SPK-002', 11, 1, 'A1.101');

-- Camera Instances
INSERT INTO device_instance (instance_code, model_id, status_id, current_location) VALUES
('CAM-001', 12, 1, 'A1.101'),
('CAM-002', 13, 1, 'A1.101');


