INSERT INTO options (name, sizex, sizez)
	VALUES('Hall1Manual', 5, 12);
	
SELECT * FROM options;

INSERT INTO contents (onum, title, pos_x, pos_z, combined_pos, image_desc)
	VALUES(1, 'Картина 1', 0, 5, '0_5', 'Описание');
	
SELECT * FROM contents;

INSERT INTO options (name, sizex, sizez)
	VALUES('Hall2Manual', 7, 10);
	
DELETE FROM options
	WHERE name = 'Hall2Manual';
	
INSERT INTO options (name, sizex, sizez)
	VALUES('Hall2Manual', 7, 12);
		
SELECT c.cnum, c.title, c.image_desc, c.image_url, c.pos_x, c.pos_z, c.combined_pos, c.type
	FROM public.options AS o
	JOIN public.contents AS c ON o.onum = c.onum;
	
INSERT INTO options (name, sizex, sizez)
	VALUES('Hall2Manual', 7, 10);
	
DELETE FROM options
	WHERE name = 'Hall2Manual';
	
INSERT INTO options (name, sizex, sizez)
	VALUES('Hall2Manual', 7, 12);
	
SELECT c.cnum, c.title, c.image_desc, c.image_url, c.pos_x, c.pos_z, c.combined_pos, c.type
	FROM public.options AS o
	JOIN public.contents AS c ON o.onum = c.onum;
	
SELECT * FROM options;

INSERT INTO options (name, sizex, sizez, date_begin, date_end)
	VALUES('DateTestManual2', 7, 12, '26/09/2022 16:00', CURRENT_TIMESTAMP);
	
DELETE FROM options
	WHERE name = 'DateTestManual2';
	
show datestyle;

set datestyle to DMY;

SELECT * FROM options;
SELECT * FROM contents;
DELETE FROM options
	WHERE onum = 11;
	
DELETE FROM contents
	WHERE onum = 4;
	
UPDATE options 
	SET sizex = 8
	WHERE onum = 3;

INSERT INTO contents (onum, title, pos_x, pos_z, combined_pos, image_desc)
	VALUES(4, 'Картина 1', 0, 5, '0_5', 'Описание')
	ON CONFLICT (combined_pos) DO UPDATE
	SET title = EXCLUDED.title, pos_x = EXCLUDED.pos_x, pos_z = EXCLUDED.pos_z,
	combined_pos = EXCLUDED.combined_pos, image_desc = EXCLUDED.image_desc;

UPDATE options
    SET is_maintained = true, is_hidden = true
    WHERE onum = 4;
	
SELECT * FROM users;

INSERT INTO users(name, email, password)
 VALUES ('Газизулин Марат Русланович', 'maratg2develop@gmail.com', '080502Asd');
 
UPDATE users
	SET is_activated = true
	WHERE email = 'maratg2develop@gmail.com';
	
SELECT * FROM contents;

DELETE FROM contents
	WHERE onum = 3 AND cnum = 274;
	
INSERT INTO contents (onum, title, image_url, pos_x, pos_z, combined_pos, image_desc, type, operation)
                                VALUES(3, 'tit', 'url', 0, 5, '0_5', 'desc', 1, 'INSERT')
                                ON CONFLICT ON CONSTRAINT combined_pos_onum_unique DO UPDATE
                                SET title = EXCLUDED.title, image_url = EXCLUDED.image_url, pos_x = EXCLUDED.pos_x, pos_z = EXCLUDED.pos_z,
                               	combined_pos = EXCLUDED.combined_pos, image_desc = EXCLUDED.image_desc, type = EXCLUDED.type, operation = 'UPDATE';
								
pg_dump -U your_user your_database -t your_table --schema-only

CREATE OR REPLACE FUNCTION public.generate_create_table_statement(p_table_name character varying)
  RETURNS SETOF text AS
$BODY$
DECLARE
    v_table_ddl   text;
    column_record record;
    table_rec record;
    constraint_rec record;
    firstrec boolean;
BEGIN
    FOR table_rec IN
        SELECT c.relname FROM pg_catalog.pg_class c
            LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                WHERE relkind = 'r'
                AND relname~ ('^('||p_table_name||')$')
                AND n.nspname <> 'pg_catalog'
                AND n.nspname <> 'information_schema'
                AND n.nspname !~ '^pg_toast'
                AND pg_catalog.pg_table_is_visible(c.oid)
          ORDER BY c.relname
    LOOP

        FOR column_record IN 
            SELECT 
                b.nspname as schema_name,
                b.relname as table_name,
                a.attname as column_name,
                pg_catalog.format_type(a.atttypid, a.atttypmod) as column_type,
                CASE WHEN 
                    (SELECT substring(pg_catalog.pg_get_expr(d.adbin, d.adrelid) for 128)
                     FROM pg_catalog.pg_attrdef d
                     WHERE d.adrelid = a.attrelid AND d.adnum = a.attnum AND a.atthasdef) IS NOT NULL THEN
                    'DEFAULT '|| (SELECT substring(pg_catalog.pg_get_expr(d.adbin, d.adrelid) for 128)
                                  FROM pg_catalog.pg_attrdef d
                                  WHERE d.adrelid = a.attrelid AND d.adnum = a.attnum AND a.atthasdef)
                ELSE
                    ''
                END as column_default_value,
                CASE WHEN a.attnotnull = true THEN 
                    'NOT NULL'
                ELSE
                    'NULL'
                END as column_not_null,
                a.attnum as attnum,
                e.max_attnum as max_attnum
            FROM 
                pg_catalog.pg_attribute a
                INNER JOIN 
                 (SELECT c.oid,
                    n.nspname,
                    c.relname
                  FROM pg_catalog.pg_class c
                       LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                  WHERE c.relname = table_rec.relname
                    AND pg_catalog.pg_table_is_visible(c.oid)
                  ORDER BY 2, 3) b
                ON a.attrelid = b.oid
                INNER JOIN 
                 (SELECT 
                      a.attrelid,
                      max(a.attnum) as max_attnum
                  FROM pg_catalog.pg_attribute a
                  WHERE a.attnum > 0 
                    AND NOT a.attisdropped
                  GROUP BY a.attrelid) e
                ON a.attrelid=e.attrelid
            WHERE a.attnum > 0 
              AND NOT a.attisdropped
            ORDER BY a.attnum
        LOOP
            IF column_record.attnum = 1 THEN
                v_table_ddl:='CREATE TABLE '||column_record.schema_name||'.'||column_record.table_name||' (';
            ELSE
                v_table_ddl:=v_table_ddl||',';
            END IF;

            IF column_record.attnum <= column_record.max_attnum THEN
                v_table_ddl:=v_table_ddl||chr(10)||
                         '    '||column_record.column_name||' '||column_record.column_type||' '||column_record.column_default_value||' '||column_record.column_not_null;
            END IF;
        END LOOP;

        firstrec := TRUE;
        FOR constraint_rec IN
            SELECT conname, pg_get_constraintdef(c.oid) as constrainddef 
                FROM pg_constraint c 
                    WHERE conrelid=(
                        SELECT attrelid FROM pg_attribute
                        WHERE attrelid = (
                            SELECT oid FROM pg_class WHERE relname = table_rec.relname
                        ) AND attname='tableoid'
                    )
        LOOP
            v_table_ddl:=v_table_ddl||','||chr(10);
            v_table_ddl:=v_table_ddl||'CONSTRAINT '||constraint_rec.conname;
            v_table_ddl:=v_table_ddl||chr(10)||'    '||constraint_rec.constrainddef;
            firstrec := FALSE;
        END LOOP;
        v_table_ddl:=v_table_ddl||');';
        RETURN NEXT v_table_ddl;
    END LOOP;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.generate_create_table_statement(character varying)
  OWNER TO postgres;
  
SELECT * FROM generate_create_table_statement('.*');