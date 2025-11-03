select * from [user];

select * from device_instance;

select * from request_status;

select * from borrow_request;

DECLARE @RequestId INT;

INSERT INTO 
    borrow_request (user_id, instance_id, request_date, usage_location, status_id)
    
VALUES (5, 5, '2025-10-17', 'A32', 1);

SET @RequestId = SCOPE_IDENTITY();

SELECT
	br.request_id AS Id,
    u.fullname AS Borrower,
	di.instance_code AS InstanceCode,
	dm.model_name AS DeviceName,
	br.request_date AS RequestDate,
	br.usage_location AS UsageLocation


FROM
    borrow_request br

JOIN [user] u ON br.user_id = u.user_id

JOIN device_instance di ON br.instance_id = di.instance_id

JOIN device_model dm ON dm.model_id = di.model_id

WHERE 
	br.request_id = 1;