# Tokyo AI 组织架构数据库设计文档

## 概述
基于Tokyo AI组织结构图设计的PostgreSQL数据库架构,用于管理组织的部门、职位、员工及其层级关系。

**数据库名称**: `TokyoAI`  
**创建日期**: 2025年10月22日  
**数据库类型**: PostgreSQL

---

## 数据库表结构

### 1. departments (部门表)
存储Tokyo AI的所有部门信息,支持多层级部门结构。

| 字段名 | 数据类型 | 约束 | 说明 |
|--------|---------|------|------|
| department_id | SERIAL | PRIMARY KEY | 部门ID(主键) |
| department_name | VARCHAR(100) | NOT NULL, UNIQUE | 部门名称 |
| department_code | VARCHAR(50) | UNIQUE | 部门编码 |
| description | TEXT | - | 部门描述 |
| parent_department_id | INTEGER | FK(departments) | 上级部门ID |
| level | INTEGER | NOT NULL, DEFAULT 1 | 部门层级(1为顶层) |
| is_active | BOOLEAN | DEFAULT true | 是否启用 |
| created_at | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | 更新时间 |

**索引**:
- `idx_departments_parent` on `parent_department_id`
- `idx_departments_level` on `level`

**触发器**:
- `trigger_departments_updated_at` - 自动更新 `updated_at` 字段

---

### 2. positions (职位表)
存储公司的职位信息,包括职位级别和薪资范围。

| 字段名 | 数据类型 | 约束 | 说明 |
|--------|---------|------|------|
| position_id | SERIAL | PRIMARY KEY | 职位ID(主键) |
| position_name | VARCHAR(100) | NOT NULL, UNIQUE | 职位名称 |
| position_code | VARCHAR(50) | UNIQUE | 职位编码 |
| position_level | INTEGER | NOT NULL | 职位级别 |
| description | TEXT | - | 职位描述 |
| min_salary | DECIMAL(15,2) | - | 最低薪资 |
| max_salary | DECIMAL(15,2) | - | 最高薪资 |
| is_active | BOOLEAN | DEFAULT true | 是否启用 |
| created_at | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | 更新时间 |

**索引**:
- `idx_positions_level` on `position_level`

**触发器**:
- `trigger_positions_updated_at` - 自动更新 `updated_at` 字段

---

### 3. employees (员工表)
存储员工的详细信息,包括个人资料、所属部门、职位和上级关系。

| 字段名 | 数据类型 | 约束 | 说明 |
|--------|---------|------|------|
| employee_id | SERIAL | PRIMARY KEY | 员工ID(主键) |
| employee_code | VARCHAR(50) | NOT NULL, UNIQUE | 员工编号 |
| first_name | VARCHAR(50) | NOT NULL | 名 |
| last_name | VARCHAR(50) | NOT NULL | 姓 |
| full_name | VARCHAR(100) | GENERATED | 全名(自动生成) |
| email | VARCHAR(100) | NOT NULL, UNIQUE | 电子邮箱 |
| phone | VARCHAR(20) | - | 电话号码 |
| department_id | INTEGER | NOT NULL, FK(departments) | 所属部门ID |
| position_id | INTEGER | NOT NULL, FK(positions) | 职位ID |
| manager_id | INTEGER | FK(employees) | 直接上级ID |
| hire_date | DATE | NOT NULL, DEFAULT CURRENT_DATE | 入职日期 |
| birth_date | DATE | - | 出生日期 |
| gender | VARCHAR(10) | CHECK | 性别 (Male/Female/Other) |
| address | TEXT | - | 地址 |
| salary | DECIMAL(15,2) | - | 薪资 |
| status | VARCHAR(20) | CHECK, DEFAULT 'Active' | 员工状态 |
| notes | TEXT | - | 备注 |
| created_at | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | 创建时间 |
| updated_at | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | 更新时间 |

**状态值**:
- Active (在职)
- Inactive (非活跃)
- On Leave (休假)
- Terminated (离职)

**索引**:
- `idx_employees_department` on `department_id`
- `idx_employees_position` on `position_id`
- `idx_employees_manager` on `manager_id`
- `idx_employees_email` on `email`
- `idx_employees_status` on `status`

**触发器**:
- `trigger_employees_updated_at` - 自动更新 `updated_at` 字段

---

### 4. organization_hierarchy (组织层级关系表)
使用闭包表模式存储部门间的所有层级关系,便于快速查询组织结构。

| 字段名 | 数据类型 | 约束 | 说明 |
|--------|---------|------|------|
| hierarchy_id | SERIAL | PRIMARY KEY | 层级关系ID(主键) |
| ancestor_dept_id | INTEGER | NOT NULL, FK(departments) | 祖先部门ID |
| descendant_dept_id | INTEGER | NOT NULL, FK(departments) | 后代部门ID |
| depth | INTEGER | NOT NULL | 层级深度(0表示自身) |
| path | VARCHAR(500) | - | 部门路径 |

**唯一约束**:
- (ancestor_dept_id, descendant_dept_id)

**索引**:
- `idx_org_hierarchy_ancestor` on `ancestor_dept_id`
- `idx_org_hierarchy_descendant` on `descendant_dept_id`
- `idx_org_hierarchy_depth` on `depth`

---

## 视图 (Views)

### 1. v_employee_organization (员工组织视图)
综合展示员工的完整组织信息,包括部门、职位和上级。

**字段**:
- employee_id, employee_code, employee_name
- email, phone
- department_name, department_code
- position_name, position_level
- manager_name, manager_email
- hire_date, status, salary

**示例查询**:
```sql
SELECT * FROM v_employee_organization WHERE status = 'Active';
```

---

### 2. v_department_tree (部门树形结构视图)
使用递归CTE展示部门的完整树形结构,包括路径和员工数量。

**字段**:
- department_id, department_name, department_code
- parent_department_id, level
- full_path (完整路径,如: "Tokyo AI > 研发部 > 前端组")
- id_path (ID路径)
- employee_count (部门员工数)

**示例查询**:
```sql
SELECT * FROM v_department_tree ORDER BY full_path;
```

---

## 函数和触发器

### update_updated_at_column()
自动更新记录的 `updated_at` 字段为当前时间戳。

**应用于**:
- departments 表
- positions 表
- employees 表

---

## 关系图

```
departments (部门)
    ↓ (1:N)
employees (员工) ← (1:1) positions (职位)
    ↓ (self-reference)
employees (上级)

departments
    ↓ (N:N)
organization_hierarchy (闭包表)
```

---

## 常用查询示例

### 查询所有在职员工及其组织信息
```sql
SELECT * 
FROM v_employee_organization 
WHERE status = 'Active'
ORDER BY department_name, position_level;
```

### 查询某个部门的所有员工
```sql
SELECT 
    e.full_name, 
    p.position_name, 
    e.email,
    e.hire_date
FROM employees e
JOIN positions p ON e.position_id = p.position_id
JOIN departments d ON e.department_id = d.department_id
WHERE d.department_name = '研发部'
  AND e.status = 'Active';
```

### 查询某个员工的所有下属(递归)
```sql
WITH RECURSIVE subordinates AS (
    -- 起点:指定的员工
    SELECT 
        employee_id, 
        full_name, 
        manager_id, 
        1 as level
    FROM employees
    WHERE employee_id = 1  -- 替换为实际的员工ID
    
    UNION ALL
    
    -- 递归:查找下属
    SELECT 
        e.employee_id, 
        e.full_name, 
        e.manager_id, 
        s.level + 1
    FROM employees e
    INNER JOIN subordinates s ON e.manager_id = s.employee_id
)
SELECT * FROM subordinates ORDER BY level, full_name;
```

### 查询部门层级结构
```sql
SELECT 
    REPEAT('  ', level - 1) || department_name AS hierarchy,
    department_code,
    employee_count
FROM v_department_tree
ORDER BY id_path;
```

### 统计各部门员工数量
```sql
SELECT 
    d.department_name,
    COUNT(e.employee_id) as employee_count,
    AVG(e.salary) as avg_salary
FROM departments d
LEFT JOIN employees e ON d.department_id = e.department_id
WHERE e.status = 'Active' OR e.status IS NULL
GROUP BY d.department_id, d.department_name
ORDER BY employee_count DESC;
```

---

## 数据完整性约束

1. **外键约束**:
   - employees.department_id → departments.department_id
   - employees.position_id → positions.position_id
   - employees.manager_id → employees.employee_id
   - departments.parent_department_id → departments.department_id
   - organization_hierarchy 的祖先和后代字段 → departments.department_id

2. **唯一约束**:
   - 部门名称、部门编码
   - 职位名称、职位编码
   - 员工编号、员工邮箱

3. **检查约束**:
   - 员工性别只能是: Male, Female, Other
   - 员工状态只能是: Active, Inactive, On Leave, Terminated

---

## 性能优化建议

1. **索引已创建**:
   - 所有外键字段都有索引
   - 常用查询字段(email, status, level)都有索引

2. **分区建议**(当数据量大时):
   - employees 表可按 hire_date 进行范围分区
   - 可考虑按 status 进行列表分区

3. **查询优化**:
   - 使用视图简化常用查询
   - 使用闭包表优化层级查询
   - 避免在 full_name(生成列)上进行复杂计算

---

## 备份和维护

### 定期备份
```bash
pg_dump -U postgres -d TokyoAI -F c -f tokyoai_backup_$(date +%Y%m%d).dump
```

### 恢复备份
```bash
pg_restore -U postgres -d TokyoAI tokyoai_backup_20251022.dump
```

### 更新统计信息
```sql
ANALYZE departments;
ANALYZE positions;
ANALYZE employees;
ANALYZE organization_hierarchy;
```

---

## 扩展建议

未来可以考虑添加以下表:

1. **employee_history** - 员工变更历史
2. **attendance** - 考勤记录
3. **performance_reviews** - 绩效评估
4. **training_records** - 培训记录
5. **department_budget** - 部门预算
6. **projects** - 项目管理
7. **employee_skills** - 员工技能

---

## 文件清单

- `TokyoAI_Database_Schema.sql` - 完整的建表SQL脚本
- `TokyoAI_Database_Documentation.md` - 本文档

---

**创建者**: GitHub Copilot  
**最后更新**: 2025年10月22日
