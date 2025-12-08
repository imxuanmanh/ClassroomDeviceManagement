USE ClassroomDeviceManagement;

ALTER TABLE borrow_request
ADD start_time DATETIME NOT NULL,
    end_time DATETIME NOT NULL;

ALTER TABLE borrow_request
DROP COLUMN start_time, end_time;

ALTER TABLE borrow_request
ADD start_period TINYINT NOT NULL,
    end_period TINYINT NOT NULL;

ALTER TABLE borrow_request
ADD CONSTRAINT chk_start_end CHECK (start_period < end_period);

INSERT INTO report_status (name) VALUES ('Cancel');

CREATE TABLE notifications (
    id INT IDENTITY(1,1) PRIMARY KEY,  
    user_id INT NOT NULL,              
    content TEXT NOT NULL,             
    seen BIT NOT NULL DEFAULT 0,       
    deleted BIT NOT NULL DEFAULT 0     
);

CREATE INDEX idx_user_id ON notifications(user_id);

ALTER TABLE notifications
ALTER COLUMN content NVARCHAR(MAX) NOT NULL;


INSERT INTO notifications (user_id, content, seen, deleted) VALUES (3, N'Yêu cầu mượn đã được chấp nhận', 0, 0);
INSERT INTO notifications (user_id, content, seen, deleted) VALUES (3, N'Yêu cầu mượn thiết bị Canon EOS 200D (CAM-001) đã được chấp nhận', 0, 0);



                
                

