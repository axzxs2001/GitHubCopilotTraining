-- =============================================
-- Tokyo AI 组织架构数据库表结构
-- 数据库: TokyoAI
-- 创建日期: 2025-10-22
-- 说明: 基于Tokyo AI组织结构图创建的完整数据库架构
-- =============================================

-- =============================================
-- 1. 部门表 (departments)
-- =============================================
CREATE TABLE IF NOT EXISTS departments (
    department_id SERIAL PRIMARY KEY,
    department_name VARCHAR(100) NOT NULL UNIQUE,
    department_code VARCHAR(50) UNIQUE,
    description TEXT,
    parent_department_id INTEGER REFERENCES departments(department_id),
    level INTEGER NOT NULL DEFAULT 1,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 添加索引以提高查询性能
CREATE INDEX IF NOT EXISTS idx_departments_parent 
    ON departments(parent_department_id);
CREATE INDEX IF NOT EXISTS idx_departments_level 
    ON departments(level);

-- 添加注释
COMMENT ON TABLE departments IS 'Tokyo AI组织部门信息表';
COMMENT ON COLUMN departments.department_id IS '部门ID(主键)';
COMMENT ON COLUMN departments.department_name IS '部门名称';
COMMENT ON COLUMN departments.department_code IS '部门编码';
COMMENT ON COLUMN departments.description IS '部门描述';
COMMENT ON COLUMN departments.parent_department_id IS '上级部门ID';
COMMENT ON COLUMN departments.level IS '部门层级(1为顶层)';
COMMENT ON COLUMN departments.is_active IS '是否启用';
COMMENT ON COLUMN departments.created_at IS '创建时间';
COMMENT ON COLUMN departments.updated_at IS '更新时间';

-- =============================================
-- 2. 职位表 (positions)
-- =============================================
CREATE TABLE IF NOT EXISTS positions (
    position_id SERIAL PRIMARY KEY,
    position_name VARCHAR(100) NOT NULL UNIQUE,
    position_code VARCHAR(50) UNIQUE,
    position_level INTEGER NOT NULL,
    description TEXT,
    min_salary DECIMAL(15, 2),
    max_salary DECIMAL(15, 2),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 添加索引
CREATE INDEX IF NOT EXISTS idx_positions_level 
    ON positions(position_level);

-- 添加注释
COMMENT ON TABLE positions IS 'Tokyo AI职位信息表';
COMMENT ON COLUMN positions.position_id IS '职位ID(主键)';
COMMENT ON COLUMN positions.position_name IS '职位名称';
COMMENT ON COLUMN positions.position_code IS '职位编码';
COMMENT ON COLUMN positions.position_level IS '职位级别';
COMMENT ON COLUMN positions.description IS '职位描述';
COMMENT ON COLUMN positions.min_salary IS '最低薪资';
COMMENT ON COLUMN positions.max_salary IS '最高薪资';
COMMENT ON COLUMN positions.is_active IS '是否启用';
COMMENT ON COLUMN positions.created_at IS '创建时间';
COMMENT ON COLUMN positions.updated_at IS '更新时间';

-- =============================================
-- 3. 员工表 (employees)
-- =============================================
CREATE TABLE IF NOT EXISTS employees (
    employee_id SERIAL PRIMARY KEY,
    employee_code VARCHAR(50) NOT NULL UNIQUE,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    full_name VARCHAR(100) GENERATED ALWAYS AS (first_name || ' ' || last_name) STORED,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20),
    department_id INTEGER NOT NULL REFERENCES departments(department_id),
    position_id INTEGER NOT NULL REFERENCES positions(position_id),
    manager_id INTEGER REFERENCES employees(employee_id),
    hire_date DATE NOT NULL DEFAULT CURRENT_DATE,
    birth_date DATE,
    gender VARCHAR(10) CHECK (gender IN ('Male', 'Female', 'Other')),
    address TEXT,
    salary DECIMAL(15, 2),
    status VARCHAR(20) DEFAULT 'Active' CHECK (status IN ('Active', 'Inactive', 'On Leave', 'Terminated')),
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 添加索引以提高查询性能
CREATE INDEX IF NOT EXISTS idx_employees_department 
    ON employees(department_id);
CREATE INDEX IF NOT EXISTS idx_employees_position 
    ON employees(position_id);
CREATE INDEX IF NOT EXISTS idx_employees_manager 
    ON employees(manager_id);
CREATE INDEX IF NOT EXISTS idx_employees_email 
    ON employees(email);
CREATE INDEX IF NOT EXISTS idx_employees_status 
    ON employees(status);

-- 添加注释
COMMENT ON TABLE employees IS 'Tokyo AI员工信息表';
COMMENT ON COLUMN employees.employee_id IS '员工ID(主键)';
COMMENT ON COLUMN employees.employee_code IS '员工编号';
COMMENT ON COLUMN employees.first_name IS '名';
COMMENT ON COLUMN employees.last_name IS '姓';
COMMENT ON COLUMN employees.full_name IS '全名(自动生成)';
COMMENT ON COLUMN employees.email IS '电子邮箱';
COMMENT ON COLUMN employees.phone IS '电话号码';
COMMENT ON COLUMN employees.department_id IS '所属部门ID';
COMMENT ON COLUMN employees.position_id IS '职位ID';
COMMENT ON COLUMN employees.manager_id IS '直接上级ID';
COMMENT ON COLUMN employees.hire_date IS '入职日期';
COMMENT ON COLUMN employees.birth_date IS '出生日期';
COMMENT ON COLUMN employees.gender IS '性别';
COMMENT ON COLUMN employees.address IS '地址';
COMMENT ON COLUMN employees.salary IS '薪资';
COMMENT ON COLUMN employees.status IS '员工状态';
COMMENT ON COLUMN employees.notes IS '备注';
COMMENT ON COLUMN employees.created_at IS '创建时间';
COMMENT ON COLUMN employees.updated_at IS '更新时间';

-- =============================================
-- 4. 组织层级关系表 (organization_hierarchy)
-- =============================================
CREATE TABLE IF NOT EXISTS organization_hierarchy (
    hierarchy_id SERIAL PRIMARY KEY,
    ancestor_dept_id INTEGER NOT NULL REFERENCES departments(department_id),
    descendant_dept_id INTEGER NOT NULL REFERENCES departments(department_id),
    depth INTEGER NOT NULL,
    path VARCHAR(500),
    UNIQUE (ancestor_dept_id, descendant_dept_id)
);

-- 添加索引
CREATE INDEX IF NOT EXISTS idx_org_hierarchy_ancestor 
    ON organization_hierarchy(ancestor_dept_id);
CREATE INDEX IF NOT EXISTS idx_org_hierarchy_descendant 
    ON organization_hierarchy(descendant_dept_id);
CREATE INDEX IF NOT EXISTS idx_org_hierarchy_depth 
    ON organization_hierarchy(depth);

-- 添加注释
COMMENT ON TABLE organization_hierarchy IS 'Tokyo AI组织层级关系表(闭包表)';
COMMENT ON COLUMN organization_hierarchy.hierarchy_id IS '层级关系ID(主键)';
COMMENT ON COLUMN organization_hierarchy.ancestor_dept_id IS '祖先部门ID';
COMMENT ON COLUMN organization_hierarchy.descendant_dept_id IS '后代部门ID';
COMMENT ON COLUMN organization_hierarchy.depth IS '层级深度(0表示自身)';
COMMENT ON COLUMN organization_hierarchy.path IS '部门路径';

-- =============================================
-- 5. 创建更新时间自动触发器
-- =============================================
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION update_updated_at_column() IS '自动更新updated_at字段的触发器函数';

-- 为departments表添加触发器
CREATE TRIGGER trigger_departments_updated_at
    BEFORE UPDATE ON departments
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- 为positions表添加触发器
CREATE TRIGGER trigger_positions_updated_at
    BEFORE UPDATE ON positions
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- 为employees表添加触发器
CREATE TRIGGER trigger_employees_updated_at
    BEFORE UPDATE ON employees
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- =============================================
-- 6. 创建视图
-- =============================================

-- 员工组织视图 - 方便查询员工的完整组织信息
CREATE OR REPLACE VIEW v_employee_organization AS
SELECT 
    e.employee_id,
    e.employee_code,
    e.full_name AS employee_name,
    e.email,
    e.phone,
    d.department_name,
    d.department_code,
    p.position_name,
    p.position_level,
    m.full_name AS manager_name,
    m.email AS manager_email,
    e.hire_date,
    e.status,
    e.salary
FROM 
    employees e
    LEFT JOIN departments d ON e.department_id = d.department_id
    LEFT JOIN positions p ON e.position_id = p.position_id
    LEFT JOIN employees m ON e.manager_id = m.employee_id;

COMMENT ON VIEW v_employee_organization IS 'Tokyo AI员工组织架构视图 - 包含员工、部门、职位和上级信息';

-- 部门层级视图 - 方便查询部门树形结构
CREATE OR REPLACE VIEW v_department_tree AS
WITH RECURSIVE dept_tree AS (
    -- 根部门
    SELECT 
        department_id,
        department_name,
        department_code,
        parent_department_id,
        level,
        CAST(department_name AS VARCHAR(500)) AS full_path,
        CAST(department_id::VARCHAR AS VARCHAR(500)) AS id_path
    FROM departments
    WHERE parent_department_id IS NULL
    
    UNION ALL
    
    -- 递归查询子部门
    SELECT 
        d.department_id,
        d.department_name,
        d.department_code,
        d.parent_department_id,
        d.level,
        CAST(dt.full_path || ' > ' || d.department_name AS VARCHAR(500)),
        CAST(dt.id_path || '/' || d.department_id::VARCHAR AS VARCHAR(500))
    FROM departments d
    INNER JOIN dept_tree dt ON d.parent_department_id = dt.department_id
)
SELECT 
    department_id,
    department_name,
    department_code,
    parent_department_id,
    level,
    full_path,
    id_path,
    (SELECT COUNT(*) FROM employees WHERE department_id = dept_tree.department_id) AS employee_count
FROM dept_tree
ORDER BY id_path;

COMMENT ON VIEW v_department_tree IS 'Tokyo AI部门树形结构视图 - 包含完整路径和员工数量';

-- =============================================
-- 7. 示例数据插入 (可选)
-- =============================================

-- 插入示例部门数据
-- INSERT INTO departments (department_name, department_code, parent_department_id, level, description) VALUES
-- ('Tokyo AI', 'TA-000', NULL, 1, 'Tokyo AI总部'),
-- ('研发部', 'TA-RD', 1, 2, '研发部门'),
-- ('市场部', 'TA-MKT', 1, 2, '市场部门'),
-- ('人力资源部', 'TA-HR', 1, 2, '人力资源部门'),
-- ('财务部', 'TA-FIN', 1, 2, '财务部门');

-- 插入示例职位数据
-- INSERT INTO positions (position_name, position_code, position_level, min_salary, max_salary, description) VALUES
-- ('CEO', 'POS-001', 1, 500000.00, 1000000.00, '首席执行官'),
-- ('CTO', 'POS-002', 2, 400000.00, 800000.00, '首席技术官'),
-- ('研发经理', 'POS-003', 3, 200000.00, 400000.00, '研发部门经理'),
-- ('高级工程师', 'POS-004', 4, 150000.00, 300000.00, '高级软件工程师'),
-- ('工程师', 'POS-005', 5, 80000.00, 150000.00, '软件工程师');

-- 插入示例员工数据
-- INSERT INTO employees (employee_code, first_name, last_name, email, department_id, position_id, manager_id, hire_date, gender, status) VALUES
-- ('EMP-001', 'Taro', 'Yamada', 'yamada@tokyoai.com', 1, 1, NULL, '2020-01-01', 'Male', 'Active'),
-- ('EMP-002', 'Hanako', 'Sato', 'sato@tokyoai.com', 2, 2, 1, '2020-02-01', 'Female', 'Active'),
-- ('EMP-003', 'Jiro', 'Tanaka', 'tanaka@tokyoai.com', 2, 3, 2, '2020-03-01', 'Male', 'Active');

-- =============================================
-- 8. 常用查询示例
-- =============================================

-- 查询所有员工及其组织信息
-- SELECT * FROM v_employee_organization WHERE status = 'Active';

-- 查询部门树形结构
-- SELECT * FROM v_department_tree;

-- 查询某个部门的所有员工
-- SELECT e.full_name, p.position_name, e.email 
-- FROM employees e
-- JOIN positions p ON e.position_id = p.position_id
-- WHERE e.department_id = 1;

-- 查询某个员工的所有下属
-- WITH RECURSIVE subordinates AS (
--     SELECT employee_id, full_name, manager_id, 1 as level
--     FROM employees
--     WHERE employee_id = 1
--     UNION ALL
--     SELECT e.employee_id, e.full_name, e.manager_id, s.level + 1
--     FROM employees e
--     INNER JOIN subordinates s ON e.manager_id = s.employee_id
-- )
-- SELECT * FROM subordinates;
