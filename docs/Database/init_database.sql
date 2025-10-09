-- ========================================
-- CLASSROOM DEVICE MANAGEMENT SYSTEM
-- ========================================
-- Chuyển sang DB master để thoát khỏi DB đang muốn xóa vì không thể xóa database khi đang kết nối trong chính nó
USE master;
GO

-- Xóa Database cũ (nếu có) và tạo mới lại
DROP DATABASE IF EXISTS ClassroomDeviceManagement;
GO

CREATE DATABASE ClassroomDeviceManagement;
GO

USE ClassroomDeviceManagement;
GO
-- ========================================
-- 1. LOOKUP TABLES (Bảng tra cứu)
-- ========================================

-- Bảng danh mục thiết bị
CREATE TABLE device_category (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    
    -- Constraints
    CONSTRAINT chk_category_name_not_empty 
        CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT uk_category_name UNIQUE (name)
);

-- Bảng vai trò người dùng
CREATE TABLE user_role (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL,
    
    -- Constraints
    CONSTRAINT chk_role_name_not_empty 
        CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT uk_role_name UNIQUE (name)
);

-- Bảng trạng thái instance
CREATE TABLE instance_status (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL,
    
    -- Constraints
    CONSTRAINT chk_instance_status_name_not_empty 
        CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT uk_instance_status_name UNIQUE (name)
);

-- Bảng trạng thái yêu cầu
CREATE TABLE request_status (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL,
    
    -- Constraints
    CONSTRAINT chk_request_status_name_not_empty 
        CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT uk_request_status_name UNIQUE (name)
);

-- Bảng trạng thái báo cáo
CREATE TABLE report_status (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL,
    
    -- Constraints
    CONSTRAINT chk_report_status_name_not_empty 
        CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT uk_report_status_name UNIQUE (name)
);

-- ========================================
-- 2. MAIN TABLES (Bảng chính)
-- ========================================

-- Bảng người dùng
CREATE TABLE [user] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    fullname NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    role_id INT NOT NULL,
    
    -- Constraints
    CONSTRAINT chk_username_valid 
        CHECK (LEN(LTRIM(RTRIM(username))) >= 3),
    CONSTRAINT chk_email_format 
        CHECK (email LIKE '%_@_%._%' AND email NOT LIKE '%@%@%'),
    CONSTRAINT chk_fullname_not_empty 
        CHECK (LEN(LTRIM(RTRIM(fullname))) > 0),
    CONSTRAINT chk_password_hash_not_empty 
        CHECK (LEN(LTRIM(RTRIM(password_hash))) > 0),
    
    -- Unique constraints
    CONSTRAINT uk_username UNIQUE (username),
    CONSTRAINT uk_email UNIQUE (email),
    
    -- Foreign key
    CONSTRAINT fk_user_role 
        FOREIGN KEY (role_id) REFERENCES user_role(id)
);

-- Bảng model thiết bị
CREATE TABLE device_model (
    model_id INT IDENTITY(1,1) PRIMARY KEY,
    model_name NVARCHAR(100) NOT NULL,
    category_id INT NOT NULL,
    specifications NVARCHAR(MAX),
    storage_location NVARCHAR(200),
    
    -- Constraints
    CONSTRAINT chk_model_name_not_empty 
        CHECK (LEN(LTRIM(RTRIM(model_name))) > 0),
    
    -- Foreign key
    CONSTRAINT fk_device_model_category 
        FOREIGN KEY (category_id) REFERENCES device_category(id)
);

-- Bảng instance thiết bị
CREATE TABLE device_instance (
    instance_id INT IDENTITY(1,1) PRIMARY KEY,
    instance_code NVARCHAR(50) NOT NULL,
    model_id INT NOT NULL,
    status_id INT NOT NULL,
    current_location NVARCHAR(200),
    
    -- Constraints
    CONSTRAINT chk_instance_code_not_empty 
        CHECK (LEN(LTRIM(RTRIM(instance_code))) > 0),
    CONSTRAINT uk_instance_code UNIQUE (instance_code),
    
    -- Foreign keys
    CONSTRAINT fk_device_instance_model 
        FOREIGN KEY (model_id) REFERENCES device_model(model_id),
    CONSTRAINT fk_device_instance_status 
        FOREIGN KEY (status_id) REFERENCES instance_status(id)
);

-- Bảng yêu cầu mượn
CREATE TABLE borrow_request (
    request_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    instance_id INT NOT NULL,
    request_date DATETIME NOT NULL DEFAULT GETDATE(),
    approved_date DATETIME,
    return_date DATETIME,
    usage_location NVARCHAR(200),
    status_id INT NOT NULL,
    reject_reason NVARCHAR(MAX),
    
    -- Date constraints
    CONSTRAINT chk_approved_after_request 
        CHECK (approved_date IS NULL OR approved_date >= request_date),
    CONSTRAINT chk_return_after_approved 
        CHECK (return_date IS NULL OR approved_date IS NULL OR return_date >= approved_date),
    
    -- Business logic constraints
    CONSTRAINT chk_reject_reason_logic 
        CHECK (
            (reject_reason IS NULL AND LEN(LTRIM(RTRIM(ISNULL(reject_reason, '')))) = 0) 
            OR 
            (reject_reason IS NOT NULL AND LEN(LTRIM(RTRIM(reject_reason))) > 0)
        ),
    
    -- Foreign keys
    CONSTRAINT fk_borrow_request_user 
        FOREIGN KEY (user_id) REFERENCES [user](user_id),
    CONSTRAINT fk_borrow_request_instance 
        FOREIGN KEY (instance_id) REFERENCES device_instance(instance_id),
    CONSTRAINT fk_borrow_request_status 
        FOREIGN KEY (status_id) REFERENCES request_status(id)
);

-- Bảng báo cáo
CREATE TABLE report (
    report_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    instance_id INT NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    image_path NVARCHAR(500),
    report_date DATETIME NOT NULL DEFAULT GETDATE(),
    status_id INT NOT NULL,
    
    -- Constraints
    CONSTRAINT chk_description_not_empty 
        CHECK (LEN(LTRIM(RTRIM(description))) > 0),
    CONSTRAINT chk_report_date_not_future 
        CHECK (report_date <= GETDATE()),
    
    -- Foreign keys
    CONSTRAINT fk_report_user 
        FOREIGN KEY (user_id) REFERENCES [user](user_id),
    CONSTRAINT fk_report_instance 
        FOREIGN KEY (instance_id) REFERENCES device_instance(instance_id),
    CONSTRAINT fk_report_status 
        FOREIGN KEY (status_id) REFERENCES report_status(id)
);

-- ========================================
-- 3. INSERT SAMPLE DATA (Dữ liệu mẫu)
-- ========================================

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