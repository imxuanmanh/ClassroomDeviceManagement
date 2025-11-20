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


                
                

