USE ClassroomDeviceManagement;

-- ==========================================================================================================
-- = Truy vấn mẫu																							=
-- ==========================================================================================================

-- Lấy ra tất cả category, sắp xếp theo id
Select * from device_category order by id;


Select * from device_model where category_id = 1;
Select * from device_model where model_id = 1


Select * from instance_status;
Delete from instance_status;
DBCC CHECKIDENT ('instance_status', RESEED, 0);

INSERT INTO device_instance (instance_code, model_id, status_id, current_location)
VALUES
('LAP003', 2, 1, 'A32'),
('LAP004', 2, 1, 'A32'),
('LAP005', 2, 2, 'A31');

select 
	category.id, 
	category.name,
	count(instance.instance_id) as total_quantity,
	count(case when instance.status_id = 1 then 1 end) as available_quantity

from 
	device_category category

join device_instance instance on category.id = instance.model_id

where 
	category.id = 1

group by 
	category.id, category.name;

SELECT * FROM device_instance WHERE model_id = 1

SELECT
	model.model_id AS id,
	model.model_name AS name,
	model.specifications AS specifications,
	model.storage_location AS storage_location,
	COUNT(instance.instance_id) as total_quantity,
	COUNT(CASE WHEN instance.status_id = 1 THEN 1 END) AS available_quantity

FROM
	device_model model

JOIN 
	device_instance instance ON model.model_id = instance.model_id

--WHERE
	--model.model_id = 2
	-- model.category_id = 1

GROUP BY
	model.model_id,
	model.model_name, 
    model.specifications, 
    model.storage_location;


SELECT id, name 
FROM device_category
WHERE id = 1;

INSERT INTO 
    device_category (name) 

OUTPUT
    inserted.id, inserted.name

VALUES
    ('Monitor');

select * from device_category;

SELECT instance_id, instance_code, status_id, current_location

FROM 
    device_instance 

WHERE 
    model_id = 1;