--Validacion de profesores, administradores y estudiantes en el Log In
CREATE OR ALTER PROCEDURE agregarProfesor @cedula varchar(20)
AS
Begin
INSERT INTO Profesor values (@cedula);
End;
Go

CREATE OR ALTER PROCEDURE agregarEstudiante @carnet varchar(20)
AS
Begin
INSERT INTO Estudiantes values (@carnet);
End;
Go

CREATE OR ALTER PROCEDURE agregarAdmin @cedula varchar(20)
AS
Begin
INSERT INTO Administrador values (@cedula);
End;
Go

--*******************************ADMINISTRADOR******************************************
--Crear curso
CREATE OR ALTER PROCEDURE crearCurso @Codigo varchar(10), @nombre varchar(50), @carrera varchar(50), @creditos int, @habilitado bit, @cedulaAdmin varchar (10)
AS
BEGIN
	If exists (select * from Curso where codigo = @Codigo)
		Begin
			RAISERROR ('El curso que intenta agregar ya fue agregado previamente',16,1);
		End;
	Else if not exists (select * from Administrador where cedula = @cedulaAdmin)
		Begin
			RAISERROR ('La cedula de administrador suministrada no existe',16,1);
		End;
	Else
		Begin
			INSERT INTO Curso values (@Codigo, @nombre, @carrera, @creditos, @habilitado, @cedulaAdmin);
		End;
END;
GO

--Ver todos los cursos que hay en la base de datos
CREATE OR ALTER PROCEDURE verCursos
AS
BEGIN
	SELECT * FROM Curso;
END;
GO

--Ver todos los cursos disponibles
CREATE OR ALTER PROCEDURE verCursosDisponibles
AS
BEGIN
	SELECT * FROM Curso where habilitado = 1;
END;
GO

--Ver todos los cursos de un semestre
CREATE OR ALTER PROCEDURE verCursosSemestre @ano int, @periodo varchar(10)
AS
BEGIN
	DECLARE @idSemestre int = (select idSemestre from Semestre where ano = @ano and periodo = @periodo);
	SELECT c.codigo, g.numeroGrupo from Semestre as s 
	inner join CursosPorSemestre as cs on s.idSemestre = cs.idSemestre
	inner join Curso as c on c.codigo = cs.codigoCurso
	inner join Grupo as g on c.codigo = g.codigoCurso
	where s.idSemestre = @idSemestre
	Group by c.codigo,g.numeroGrupo
END;
GO


--Permite ver los estudiantes de un grupo de un semestre y curso especifico
CREATE OR ALTER PROCEDURE verEstudiantesGrupo @ano int, @periodo varchar(10), @grupo int, @codigoCurso varchar (10)
AS
BEGIN
	DECLARE @idSemestre int = (select idSemestre from Semestre where ano = @ano and periodo = @periodo);
	SELECT  e.carnetEstudiante from Semestre as s 
	inner join CursosPorSemestre as cs on s.idSemestre = cs.idSemestre
	inner join Curso as c on c.codigo = cs.codigoCurso
	inner join Grupo as g on c.codigo = g.codigoCurso
	inner join EstudiantesGrupo as e on g.idGrupo = e.idGrupo
	where s.idSemestre = @idSemestre and g.numeroGrupo = @grupo and g.codigoCurso = @codigoCurso;
END;
GO


--Permite ver los profesores de un grupo de un semestre y curso especifico
CREATE OR ALTER PROCEDURE verProfesorSemestre @ano int, @periodo varchar(10), @grupo int, @codigoCurso varchar (10)
AS
BEGIN
	DECLARE @idSemestre int = (select idSemestre from Semestre where ano = @ano and periodo = @periodo);
	SELECT  p.cedulaProfesor from Semestre as s 
	inner join CursosPorSemestre as cs on s.idSemestre = cs.idSemestre
	inner join Curso as c on c.codigo = cs.codigoCurso
	inner join Grupo as g on c.codigo = g.codigoCurso
	inner join ProfesoresGrupo as p on g.idGrupo = p.idGrupo
	where s.idSemestre = @idSemestre and g.numeroGrupo = @grupo and g.codigoCurso = @codigoCurso;
END;
GO


--Ver los semestres 
CREATE OR ALTER PROCEDURE verSemestres
AS
BEGIN
	select ano, periodo from Semestre
	order by ano desc, periodo desc;
END;
GO

--Habilitar o deshabilitar un curso
CREATE OR ALTER PROCEDURE habilitar_deshabilitarCurso @codigo varchar(50)
AS
BEGIN
	update Curso set habilitado = habilitado ^ 1 where codigo = @codigo;
	IF (select habilitado from Curso where codigo = @codigo) = 0
	BEGIN
		delete CursosPorSemestre where codigoCurso = @codigo;
	END;
END;
GO


--Agrega un curso existente al semestre indicado
CREATE OR ALTER PROCEDURE agregarCursoSemestre @codigoCurso varchar (20), @anoSemestre int, @periodoSemestre varchar(10)
AS
BEGIN
	DECLARE @idSemestre int = (select idSemestre from Semestre where ano = @anoSemestre and periodo = @periodoSemestre);
	IF ((select habilitado from Curso where codigo = @codigoCurso) = 1)
	BEGIN
		If exists(select * from CursosPorSemestre where codigoCurso = @codigoCurso and idSemestre = @idSemestre )
		BEGIN
			RAISERROR ('El curso que intenta agregar ya fue agregado previamente',16,1);
		END;
		Else
		BEGIN
			DECLARE @idSem int = (select idSemestre from Semestre where ano = @anoSemestre and periodo = @periodoSemestre);
			insert into CursosPorSemestre values (@idSem, @codigoCurso);
		END;
	END;
	ELSE
	Begin
		RAISERROR ('El curso que desea agregar al semestre no se encuentra habilitado',16,1);
	End;
END;
GO

--Crea un semestre (1 para el primer semestre, 2 para el segundo semestre y V para el periodo de verano).  
CREATE OR ALTER PROCEDURE crearSemestre @ano int, @periodo varchar(10), @cedulaAdmin int
AS
Begin
INSERT INTO Semestre (ano, periodo, cedulaAdmin) values (@ano, @periodo, @cedulaAdmin);
End;
Go


--Crear carpetas
CREATE OR ALTER PROCEDURE crearCarpeta @nombre varchar(50), @codigoCurso varchar(10), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	insert into Carpetas (nombre, idGrupo) values (@nombre, @idGrupo);
END;
GO

--Eliminar carpetas
CREATE OR ALTER PROCEDURE eliminarCarpeta @nombre varchar(50), @codigoCurso varchar(10), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idCarpeta int = (select idCarpeta from Carpetas where nombre = @nombre and idGrupo = @idGrupo);
	delete from Documentos where idCarpeta = @idCarpeta;
	delete from Carpetas where nombre = @nombre and idGrupo = @idGrupo;
END;
GO

--Crear Documentos
CREATE OR ALTER PROCEDURE crearDocumentos @nombreDocumento varchar(50), @archivo varchar(MAX),@tamano int, @nombreCarpeta varchar(50),
@codigoCurso varchar(10), @numeroGrupo int, @tipoArchivo varchar (10)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	Declare @idCarpeta int = (select idCarpeta from Carpetas where nombre = @nombreCarpeta and idGrupo = @idGrupo);
	insert into Documentos(nombre, archivo, tamano, idCarpeta, tipoArchivo) values (@nombreDocumento,  @archivo, @tamano, @idCarpeta, @tipoArchivo);
	Declare @cantDocu int = (select count(*) from Documentos where idCarpeta = @idCarpeta);
	update Carpetas set tamano = @cantDocu where idCarpeta =@idCarpeta;
END;
GO

--Eliminar Documentos
CREATE OR ALTER PROCEDURE eliminarDocumentos @nombreDocumento varchar(50), @nombreCarpeta varchar(50), @codigoCurso varchar(10), @numeroGrupo int
AS
BEGIN
	Declare @idGrupo int = (select idGrupo from Grupo where numeroGrupo = @numeroGrupo and codigoCurso = @codigoCurso);
	Declare @idCarpeta int = (select idCarpeta from Carpetas where nombre = @nombreCarpeta and idGrupo = @idGrupo);
	delete from Documentos where idCarpeta = @idCarpeta and nombre = @nombreDocumento;
	update Carpetas set tamano = (SELECT COUNT (*) from Documentos where idCarpeta = @idCarpeta) where idCarpeta = @idCarpeta;
END;
GO

--Crear un rubro
CREATE OR ALTER PROCEDURE crearRubro @rubro varchar(50), @porcentaje decimal(5,2), @codigoCurso varchar (30), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	insert into Rubros(rubro, porcentaje, idGrupo) values (@rubro, @porcentaje, @idGrupo);
END;
GO

--Eliminar un rubro
CREATE OR ALTER PROCEDURE eliminarRubro @rubro varchar(50), @codigoCurso varchar (30), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idRubro int = (select idRubro from Rubros where rubro = @rubro and idGrupo = @idGrupo);
	DECLARE @idEvaluacion int = (select idEvaluacion from Evaluaciones where idRubro = @idRubro);
	delete from EvaluacionesEstudiantes where idEvaluacion = @idEvaluacion;
	delete from Evaluaciones where idEvaluacion = @idEvaluacion;
	delete from Rubros where idRubro = @idRubro;
END;
GO

--Crear Evaluacion
CREATE OR ALTER PROCEDURE crearEvaluacion @grupal int, @nombre varchar(50), @porcentaje decimal(5,2), @fechaInicio datetime, @fechaFin datetime,
@archivo varchar(MAX), @nombArch varchar (100), @tipArch varchar(100),  @rubro varchar(50), @codigoCurso varchar(20), @numeroGrupo int, @cantPersonas int
AS
BEGIN
	DECLARE @idGrup int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idRubro int = (select idRubro from Rubros where idGrupo = @idGrup and rubro = @rubro);
	insert into Evaluaciones (grupal, nombre, porcentaje, fechaInicio, fechaFin, archivo, idRubro, nombreArchivo, tipoArchivo, cantPersonas)
	values (@grupal, @nombre, @porcentaje, @fechaInicio, @fechaFin,  @archivo, @idRubro, @nombArch, @tipArch, @cantPersonas);
	IF (@grupal = 0)
	BEGIN
		DECLARE @idEva int = (select idEvaluacion from Evaluaciones where idRubro = @idRubro and nombre = @nombre);
		insert into EvaluacionesEstudiantes (carnet, idEvaluacion) 
		select eg.carnetEstudiante, e.idEvaluacion from EstudiantesGrupo as eg
		inner join Grupo as g on eg.idGrupo = g.idGrupo
		inner join Rubros as r on r.idGrupo = g.idGrupo
		inner join Evaluaciones as e on e.idRubro = r.idRubro
		where e.idEvaluacion = @idEva;
	END;
END;
GO

--Eliminar Evaluacion
CREATE OR ALTER PROCEDURE eliminarEvaluacion @nombre varchar (50), @rubro varchar(50), @codigoCurso varchar(10), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idRubro int = (select idRubro from Rubros where idGrupo = @idGrupo and rubro = @rubro);
	Delete from EvaluacionesEstudiantes where idEvaluacion = (select idEvaluacion from Evaluaciones where idRubro = @idRubro and nombre = @nombre);
	Delete from Evaluaciones where idRubro = @idRubro and nombre = @nombre;
END;
GO

--Establecer los grupos del curso y le crea las carpetas predeterminadas
CREATE OR ALTER PROCEDURE crearGrupo @codigoCurso varchar(20), @numeroGrupo int, @ano int, @periodo varchar(10)
AS
BEGIN
	DECLARE @idSemestre int = (select idSemestre from Semestre where ano = @ano and periodo = @periodo);
	IF EXISTS (select * from CursosPorSemestre where idSemestre = @idSemestre and codigoCurso = @codigoCurso)
		BEGIN
			insert into Grupo (codigoCurso, numeroGrupo) values (@codigoCurso, @numeroGrupo);
			Execute crearCarpeta @nombre = 'Presentaciones', @codigoCurso = @codigoCurso , @numeroGrupo = @numeroGrupo;
			Execute crearCarpeta @nombre = 'Quices', @codigoCurso = @codigoCurso , @numeroGrupo = @numeroGrupo;
			Execute crearCarpeta @nombre = 'Examenes', @codigoCurso = @codigoCurso , @numeroGrupo = @numeroGrupo;
			Execute crearCarpeta @nombre = 'Proyectos', @codigoCurso = @codigoCurso , @numeroGrupo = @numeroGrupo;
			Execute crearRubro @rubro = 'Quices', @porcentaje = 30, @codigoCurso = @codigoCurso, @numeroGrupo = @numeroGrupo;
			Execute crearRubro @rubro = 'Examenes', @porcentaje = 30, @codigoCurso = @codigoCurso, @numeroGrupo = @numeroGrupo;
			Execute crearRubro @rubro = 'Proyectos', @porcentaje = 40, @codigoCurso = @codigoCurso, @numeroGrupo = @numeroGrupo;
		END;
	ELSE
		RAISERROR ('El curso al que intenta agregarle un grupo no existe en este semestre',16,1);
END;
GO

 
--Establecer los profesores del grupo.
CREATE OR ALTER PROCEDURE asignarProfesorGrupo @codigoCurso varchar(10), @numeroGrupo int, @ano int, @periodo varchar (10), @cedulaProfesor varchar(20)
AS
BEGIN
	DECLARE @idSemestre int = (select idSemestre from Semestre where ano = @ano and periodo = @periodo);
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	If exists (
				select * from CursosPorSemestre as cp inner join  grupo as g
				on g.codigoCurso = cp.codigoCurso
				where idSemestre = @idSemestre and g.codigoCurso = @codigoCurso and g.numeroGrupo = @numeroGrupo)
	Begin
		insert into ProfesoresGrupo (idGrupo, cedulaProfesor) values (@idGrupo, @cedulaProfesor)
	End;
	Else
		RAISERROR ('El grupo al que intenta agregar al profesor no existe en este semestre',16,1);
END;
GO


--Eliminar profesor del grupo
CREATE OR ALTER PROCEDURE eliminarProfesorGrupo @codigoCurso varchar(10), @numeroGrupo int, @cedulaProfesor varchar(20)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	delete from ProfesoresGrupo where idGrupo = @idGrupo and cedulaProfesor = @cedulaProfesor;
END;
GO

--Establecer estudiantes del grupo
CREATE OR ALTER PROCEDURE agregarEstudiantesGrupo @carnet varchar(15), @codigoCurso varchar(10), @numeroGrupo int, @ano int, @periodo varchar(10)
AS
BEGIN
	DECLARE @idSemestre int = (select idSemestre from Semestre where ano = @ano and periodo = @periodo);
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	If exists (select * from CursosPorSemestre as cp inner join  grupo as g
				on g.codigoCurso = cp.codigoCurso
				where idSemestre = @idSemestre and g.codigoCurso = @codigoCurso and g.numeroGrupo = @numeroGrupo
				)
	Begin
		insert into EstudiantesGrupo values (@carnet, @idGrupo);
	End;
	Else
		RAISERROR ('El grupo al que intenta agregar al profesor no existe en este semestre',16,1);
END;
GO


--PROCEDURE PARA INICIALIZAR SEMESTRE EN BASE A LA TABLA DE EXCEL
CREATE OR ALTER PROCEDURE crearSemestreExcel
AS
BEGIN
	DECLARE @anio int = (select top 1 ano from Data$);
	DECLARE @period int = (select top 1 Semestre from Data$);
	--Crea el semestre
	execute crearSemestre @ano = @anio, @periodo = @period, @cedulaAdmin = 0;

	--Crea los cursos
	insert into Curso (codigo, nombre) select idCurso, NombreCurso from Data$ where IdCurso != 'NULL' group by IdCurso, NombreCurso;

	--Asigna los cursos al semestre
	insert into CursosPorSemestre select s.idSemestre, c.codigo from Curso as c inner join Data$ as d on c.codigo = d.IdCurso
										inner join Semestre as s on s.ano = d.Ano and s.periodo = d.Semestre
										group by s.idSemestre, c.codigo;

	--Crea los grupos de los estudiantes
	insert into Grupo (codigoCurso, numeroGrupo) select idCurso, Grupo from Data$ where IdCurso != 'NULL' group by idCurso, grupo;

	--Crea las carpetas y rubros por defecto de los grupos
	Declare db_cursor CURSOR for select idCurso, Grupo from Data$ where IdCurso != 'NULL' group by idCurso, grupo;
	DECLARE @cod varchar(15);
	DECLARE @num int;
	OPEN db_cursor;
	FETCH NEXT FROM db_cursor into @cod, @num
	While @@FETCH_STATUS = 0
	BEGIN
		DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @cod and numeroGrupo = @num);
		insert into Carpetas (nombre,idGrupo) values ('Presentaciones', @idGrupo);
		insert into Carpetas (nombre,idGrupo) values ('Quices', @idGrupo);
		insert into Carpetas (nombre,idGrupo) values ('Examenes', @idGrupo);
		insert into Carpetas (nombre,idGrupo) values ('Proyectos', @idGrupo);
		insert into Rubros (rubro,porcentaje, idGrupo) values ('Quices', 30, @idGrupo);
		insert into Rubros (rubro,porcentaje, idGrupo) values ('Examenes', 30, @idGrupo);
		insert into Rubros (rubro,porcentaje, idGrupo) values ('Proyectos', 40, @idGrupo);
		FETCH NEXT FROM db_cursor INTO @cod, @num;
	END;
	CLOSE db_cursor;
	DEALLOCATE db_cursor;

	--Agrega a los profesores 
	insert into Profesor select idProfesor from Data$ where idProfesor != 'NULL' group by idProfesor;

	--Agrega a los profesores a los grupos correspondientes
	insert into ProfesoresGrupo select g.idGrupo, d.idProfesor from Data$ as d inner join Profesor as p on d.IdProfesor = p.cedula
								inner join Grupo as g on g.codigoCurso = d.IdCurso and g.numeroGrupo = d.Grupo
								group by d.IdProfesor, d.Grupo, g.idGrupo;
	
	--Agrega a los estudiantes
	insert into Estudiantes select carnet from Data$ where Carnet != 'NULL' group by Carnet;

	--Agrega los estudiantes a los grupos correspondientes
	insert into EstudiantesGrupo select d.Carnet, g.idGrupo from Estudiantes as e inner join Data$ as d on d.Carnet = e.carnet
									inner join Grupo as g on g.codigoCurso = d.IdCurso and g.numeroGrupo = d.Grupo
									group by d.carnet, g.idGrupo;
END;
GO

--Obtiene los estudiantes y sus nombres del excel
CREATE OR ALTER PROCEDURE obtenerEstudiantesExcel
AS
BEGIN
select Carnet, Nombre, Apellido1,Apellido2 from  Data$ where Carnet != 'NULL' group by Carnet, Nombre, Apellido1, Apellido2
END;
GO

--Obtiene los profesores y sus datos del excel
CREATE OR ALTER PROCEDURE obtenerProfesorExcel
AS
BEGIN
select IdProfesor, NombreProfesor, ApellidoProfesor,ApellidoProfesor1 from  Data$ where IdProfesor != 'NULL' 
group by IdProfesor, NombreProfesor, ApellidoProfesor, ApellidoProfesor1
END;
GO

--Elimina la tabla temporal que se utiliza para importar el excel
CREATE OR ALTER PROCEDURE borrarTablaTemporal
AS
BEGIN
	Drop table Data$;
END;
GO

--........................................................TRIGGERS........................................................

--Valida que al crear un semestre no exista ya uno con el mismo ano y periodo
Create or Alter Trigger tr_verificarSemestre on Semestre
for Insert
As
IF Exists (select * from Semestre as s join inserted as i on s.ano = i.ano and s.periodo = i.periodo having COUNT(*)>1)
BEGIN
	RAISERROR ('El semestre que intenta crear ya existe en la base de datos.',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go

--Valida que al crear una evaluacion no se le pueda asignar un porcentaje mayor al del rubro al que corresponde
Create or Alter Trigger tr_verificarPorcentajeEvaluacion on Evaluaciones
for insert, update
AS
BEGIN
	DECLARE @porcEv decimal (5,2) = (select porcentaje from inserted);
	DECLARE @porcRub decimal (5,2) = (select porcentaje from Rubros where idRubro = (select idRubro from inserted));
	DECLARE @porcTot decimal (5,2) = (select sum (porcentaje) from Evaluaciones where idRubro = (select idRubro from inserted));
	IF (@porcEv > @porcRub)
	BEGIN
		RAISERROR ('El porcentaje de la evaluacion no puede ser mayor al del rubro al que pertenece',16,1);
		ROLLBACK TRANSACTION;
		Return
	END;
	ELSE IF(@porcTot > @porcRub)
	BEGIN
		RAISERROR ('El porcentaje total de las evaluaciones excede el porcentaje del rubro',16,1);
		ROLLBACK TRANSACTION;
		Return
	END;
END;
GO

--Valida que la carpeta que se crea no exista en el mismo grupo
Create or Alter Trigger tr_verificarCarpeta on Carpetas
for Insert
As
IF Exists (select * from Carpetas as c join inserted as i on c.nombre = i.nombre and c.idGrupo = i.idGrupo having COUNT(*)>1)
BEGIN
	RAISERROR ('La carpeta que intenta crear ya existe para ese grupo en la base de datos.',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go

--Valida que el documento que se quiere crear no exista ya en la carpeta
Create or Alter Trigger tr_verificarDocumento on Documentos
for Insert
As
IF Exists (select * from Documentos as d join inserted as i on d.nombre = i.nombre and d.idCarpeta = i.idCarpeta having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe un documento con ese nombre en esta carpeta',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go

--Valida que existan 2 evaluaciones con el mismo rubro en el mismo grupo
Create or Alter Trigger tr_verificarEvaluacion on Evaluaciones
for Insert
As
IF Exists (select * from Evaluaciones as e join inserted as i on e.idRubro = i.idRubro and e.nombre = i.nombre having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe una evaluacion con ese nombre en este grupo',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go


--Valida que no se pueda crear el mismo rubro para un grupo
Create or Alter Trigger tr_verificarRubro on Rubros
for Insert, Update
As
IF Exists (select * from Rubros as r join inserted as i on r.idGrupo = i.idGrupo and r.rubro = i.rubro having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe un rubro con ese nombre en este grupo',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
IF ((select sum (porcentaje) from Rubros where idGrupo = (select idGrupo from inserted)) > 100)
BEGIN
	RAISERROR ('El porcentaje total de los rubros excede el 100',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go


--Valida que no se puedan eliminar los rubros creados al inicializar el semestre
Create or Alter Trigger tr_EliminarRubro on Rubros
for delete
As
Declare @rubro varchar(50) = (select rubro from deleted);
If (@rubro = 'Quices' or @rubro = 'Examenes' or @rubro = 'Proyectos')
BEGIN
	RAISERROR ('No se pueden eliminar los rubros principales',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go


--Valida que no se puedan eliminar las carpetas creadas al inicializar el semestre
Create or Alter Trigger tr_EliminarCarpetas on Carpetas
for delete
As
Declare @nombreCarpeta varchar(50) = (select nombre from deleted);
If (@nombreCarpeta = 'Quices' or @nombreCarpeta = 'Examenes' or @nombreCarpeta = 'Proyectos' or @nombreCarpeta = 'Presentaciones')
BEGIN
	RAISERROR ('No se pueden eliminar las carpetas principales',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go


--........................................................TRIGGERS........................................................

--*******************************ADMINISTRADOR******************************************



--*******************************PROFESOR******************************************

--Ver Curos a los que pertenece un profesor
CREATE OR ALTER PROCEDURE verCursosProfesor @cedula varchar(20)
AS
BEGIN
	Select c.nombre, g.numeroGrupo, s.ano, s.periodo, c.codigo from ProfesoresGrupo as p
	inner join Grupo as g on g.idGrupo = p.idGrupo
	inner join Curso as c on g.codigoCurso = c.codigo 
	inner join CursosPorSemestre as cs on cs.codigoCurso = c.codigo
	inner join Semestre as s on s.idSemestre = cs.idSemestre
	where p.cedulaProfesor = @cedula
	order by s.ano desc,s.periodo desc;
END;
GO

--Ver las carpetas de un grupo
CREATE OR ALTER PROCEDURE verCarpetasGrupo @codigoCurso varchar(30), @numeroGrupo int
AS
BEGIN
DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
Select * from Carpetas where idGrupo = @idGrupo;
END;
GO

--Editar carpetas de un grupo
CREATE OR ALTER PROCEDURE editarCarpetaGrupo @nombreCarpeta varchar(50), @nuevoNombre varchar(50), @codigoCurso varchar(50), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	UPDATE Carpetas set nombre = @nuevoNombre where nombre = @nombreCarpeta and idGrupo = @idGrupo;
END;
GO

--Editar documentos (cambiar nombre)
CREATE OR ALTER PROCEDURE editarDocumentos @nombreDocumento varchar (50), @nombreCarpeta varchar(50), @codigoCurso varchar (10), @numeroGrupo int,
@nuevoNombre varchar(50)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo );
	DECLARE @idCarpeta int = (select idCarpeta from Carpetas where nombre = @nombreCarpeta and idGrupo = @idGrupo);
	Update Documentos set nombre = @nuevoNombre,  fechaSubido = getDate() where nombre = @nombreDocumento;
END;
GO

--Ver los documentos de un grupo de una carpeta especifica
CREATE OR ALTER PROCEDURE verDocumentosGrupo @nombreCarpeta varchar (50), @codigoCurso varchar (10), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo );
	DECLARE @idCarpeta int = (select idCarpeta from Carpetas where nombre = @nombreCarpeta and idGrupo = @idGrupo);
	Select * from Documentos where idCarpeta = @idCarpeta
END;
GO

--Ver los un documento especifico
CREATE OR ALTER PROCEDURE verDocumentosEspecifico @nombreCarpeta varchar (50), @codigoCurso varchar (10), @numeroGrupo int, @nombreDocumento varchar(50)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idCarpeta int = (select idCarpeta from Carpetas where nombre = @nombreCarpeta and idGrupo = @idGrupo);
	Select * from Documentos where idCarpeta = @idCarpeta and nombre = @nombreDocumento;
END;
GO

--Ver rubros de un grupo
CREATE OR ALTER PROCEDURE verRubrosGrupo @codigoCurso varchar(10), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo );
	select * from Rubros where idGrupo = @idGrupo;
END;
GO

--Editar los rubros de un grupo
CREATE OR ALTER PROCEDURE editarRubrosGrupo @codigoCurso varchar(10), @numeroGrupo int, @rubro varchar(50), @nuevoRubro varchar(50), @nuevoPorcentaje decimal(5,2)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo );
	Update Rubros set rubro = @nuevoRubro, porcentaje = @nuevoPorcentaje where idGrupo = @idGrupo and rubro = @rubro;
END;
GO

--Verificar que los rubros de un grupo sumen 100 en total
CREATE OR ALTER PROCEDURE verificarRubros @codigoCurso varchar(10), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @total decimal = (Select SUM(porcentaje) from Rubros where idGrupo = @idGrupo);
	Declare @existe int;
	If (@total != 100)
	set @existe = 0;
	Else
	set @existe = 1;
	return @existe;
END;
GO

--Editar evaluaciones de un grupo
CREATE OR ALTER PROCEDURE editarEvaluacion @nombreEvaluacion varchar (50), @codigoCurso varchar(10), @numeroGrupo int, @rubro varchar(50), 
@nuevoNombre varchar (50), @nuevaFechaInicio datetime, @nuevaFechaFin datetime, @nuevoPorcentaje decimal(5,2)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idRubro int = (select idRubro from Rubros where idGrupo = @idGrupo and rubro = @rubro);
	Update Evaluaciones set nombre = @nuevoNombre, fechaInicio = @nuevaFechaInicio, fechaFin = @nuevaFechaFin, porcentaje = @nuevoPorcentaje
	where nombre = @nombreEvaluacion and idRubro = @idRubro;
END;
GO

--Ver las evaluaciones de un grupo segun su rubro
CREATE OR ALTER PROCEDURE verEvaluacionesEstudiantes @codigoCurso varchar(10), @numeroGrupo int, @rubro varchar(50), @nombreEvaluacion varchar (50)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idRubro int = (select idRubro from Rubros where idGrupo = @idGrupo and rubro = @rubro);
	DECLARE @idEvaluacion int = (select idEvaluacion from Evaluaciones where idRubro = @idRubro and nombre = @nombreEvaluacion)
	Select carnet, archivoSolucion from EvaluacionesEstudiantes where idEvaluacion = @idEvaluacion and archivoSolucion != 'NULL'
	group by carnet, archivoSolucion
END;
GO


--Asignar grupos de trabajo
CREATE OR ALTER PROCEDURE agregarEstudianteEvaluacionGrupal @carnetEstudiante varchar (15), @idEvaluacion int, @numeroGrupoEvaluacion int
AS
BEGIN
	DECLARE @cantPermitida int = (select cantPersonas from Evaluaciones where idEvaluacion = @idEvaluacion);
	IF (select count(*) from EvaluacionesEstudiantes where idEvaluacion = @idEvaluacion and grupo = @numeroGrupoEvaluacion) < @cantPermitida
	BEGIN
		insert into EvaluacionesEstudiantes (carnet, idEvaluacion, grupo) values (@carnetEstudiante, @idEvaluacion, @numeroGrupoEvaluacion)
	END;
	ELSE
		RAISERROR ('El grupo ya esta lleno',16,1);
		Return
END;
GO

--Creacion de noticias
CREATE OR ALTER PROCEDURE crearNoticiaGrupo @codigoCurso varchar (10), @numeroGrupo int, @tituloNoticia varchar(50), @mensaje varchar(300), @cedulaAutor varchar (20)
AS
BEGIN
	Declare @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	insert into Noticias (titulo, mensaje, fecha, idGrupo,cedulaAutor) values (@tituloNoticia, @mensaje, getDate(), @idGrupo,@cedulaAutor);
END;
GO

--Eliminar noticia 
CREATE OR ALTER PROCEDURE eliminarNoticia @codigoCurso varchar (10), @numeroGrupo int, @tituloNoticia varchar(50)
AS
BEGIN
	Declare @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	delete from Noticias where idGrupo = @idGrupo and titulo = @tituloNoticia;
END;
GO

--Modificar una noticia
CREATE OR ALTER PROCEDURE editarNoticiaGrupo @codigoCurso varchar (10), @numeroGrupo int, @tituloNoticia varchar (50),
@tituloNuevo varchar (50), @mensajeNuevo varchar(300)
AS
BEGIN
	Declare @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	Update Noticias set titulo = @tituloNuevo, mensaje = @mensajeNuevo where idGrupo = @idGrupo and titulo = @tituloNoticia;
END;
GO

--Ver todas las noticias creadas por un profesor
CREATE OR ALTER PROCEDURE verNoticiasProfesor @cedula varchar(20)
AS
BEGIN
	select titulo, mensaje, fecha, c.nombre, g.numeroGrupo, g.codigoCurso from Noticias as n
	inner join grupo as g on n.idGrupo = g.idGrupo
	inner join Curso as c on c.codigo = g.codigoCurso
	where n.cedulaAutor = @cedula order by fecha desc;
END;
GO


--Ver evaluaciones de los estudiantes
CREATE OR ALTER PROCEDURE verEvaluacionesEstudiantes @idEvaluacion int
AS
BEGIN
	select * from EvaluacionesEstudiantes where idEvaluacion = @idEvaluacion;
END;
GO

--Revisar las evaluaciones estudiante, subir comentario, poner nota, subir archivo retroalimentacion)
CREATE OR ALTER PROCEDURE revisarEvaluacion @carnet varchar(20), @codigoCurso varchar(20), @numeroGrupo int, @rubro varchar (20), @nombre varchar (50),  @nota decimal(5,2), @comentario varchar(200), @nombreArch varchar(100),
@tipoArch varchar (100), @archivoRetroalimentacion varchar(MAX)
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @idRubro int = (select idRubro from Rubros where idGrupo = @idGrupo and rubro = @rubro);
	DECLARE @idEvaluacion int = (select idEvaluacion from Evaluaciones where idRubro = @idRubro and nombre = @nombre);
	update EvaluacionesEstudiantes set nota = @nota, comentario = @comentario, archivoRetroalimentacion =  @archivoRetroalimentacion, 
	nombArchRetr = @nombreArch, tipoArchRetr = @tipoArch
	where carnet = @carnet and idEvaluacion = @idEvaluacion;
END;
GO





--Indica que las notas de una evaluacion ya fueron publicadas y crea una noticia por medio de un trigger
CREATE OR ALTER PROCEDURE publicarNotas @idEvaluacion int AS
BEGIN
	IF not Exists (select * from EvaluacionesEstudiantes where idEvaluacion = @idEvaluacion and nota = NULL)
	BEGIN
		update Evaluaciones set revisado = revisado ^ 1 where idEvaluacion = @idEvaluacion;
	END;
	ELSE
	RAISERROR ('No puede publicar las notas si no estan todas revisadas',16,1);
END;
GO

--VIEW QUE PERMITE VISUALIZAR LAS NOTAS DE LOS ESTUDIANTES
CREATE OR ALTER VIEW v_notasEstudiantes
AS
	SELECT ee.carnet, e.nombre nombreEvaluacion, g.numeroGrupo, g.codigoCurso,r.rubro, ee.nota notaObtenida,
	((ee.nota*e.porcentaje)/100) porcentajeObtenido,e.porcentaje porcentajeEvaluacion
	from EvaluacionesEstudiantes as ee
	join Evaluaciones as e on ee.idEvaluacion = e.idEvaluacion
	join Rubros as r on r.idRubro = e.idRubro
	join Grupo as g on g.idGrupo = r.idGrupo
GO

--View que muestra las notas finales de un estudiante en
CREATE OR ALTER VIEW v_notasFinalesRubros
AS
select carnet, codigoCurso, rubro, sum (porcentajeObtenido) notaFinalRubro from v_notasEstudiantes group by carnet, rubro, codigoCurso
GO

--View que solo contiene el carnet y la nota total del estudiante en el curso
create or alter view v_notaTotal
AS
select carnet, codigoCurso,sum(notaFinalRubro) notaFinal from v_notasFinalesRubros group by carnet, codigoCurso;
GO


--View que permite ver los estudiantes matriculados en todos los cursos
CREATE OR ALTER VIEW v_estudiantesCursos
AS
	select carnetEstudiante, c.codigo from EstudiantesGrupo as eg 
	inner join Grupo as g on g.idGrupo = eg.idGrupo
	inner join Curso as c on c.codigo = g.codigoCurso
	group by codigo, carnetEstudiante
GO


--View notas grupales
CREATE OR ALTER VIEW v_notasGrupalesResumidas
AS
	select ne.carnet, ne.numeroGrupo, ne.codigoCurso, ne.nombreEvaluacion, ne.rubro, ne.notaObtenida, ne.porcentajeObtenido, ne.porcentajeEvaluacion, nt.notaFinal notaFinalCurso
	from v_notasEstudiantes as ne
	inner join v_notasFinalesRubros as nf on nf.carnet = ne.carnet and nf.rubro = ne.rubro
	inner join v_notaTotal as nt on nt.carnet = nf.carnet and nt.codigoCurso = nf.codigoCurso
	group by ne.carnet, ne.numeroGrupo, ne.codigoCurso, ne.rubro, ne.nombreEvaluacion, ne.notaObtenida, ne.porcentajeObtenido, ne.porcentajeEvaluacion, nt.notaFinal;
GO

--Permite ver el reporte de los estudiantes matriculados en un curso en especifico
CREATE OR ALTER PROCEDURE verEstudiantesCurso @codigoCurso varchar (20)
AS
BEGIN
select ve.carnetEstudiante, ve.codigo, s.ano, s.periodo from v_estudiantesCursos as ve
inner join CursosPorSemestre as cs on cs.codigoCurso = ve.codigo
inner join Semestre as s on s.idSemestre = cs.idSemestre
where codigo = @codigoCurso
order by s.ano desc, s.periodo desc
END;
GO


--Permite ver el reporte de notas de los estudiantes segun el grupo al que pertenezca
CREATE OR ALTER PROCEDURE verNotasGrupo @codigoCurso varchar (15), @numeroGrupo int
AS
BEGIN
	select carnet, nombreEvaluacion, rubro, notaObtenida, porcentajeObtenido, porcentajeEvaluacion, notaFinalCurso 
	from v_notasGrupalesResumidas where numeroGrupo = @numeroGrupo and codigoCurso = @codigoCurso;
END;
GO

--Ver todos los rubros y porcentajes un grupo en especifico
CREATE OR ALTER PROCEDURE verDatosAsignacion @rubro varchar(20), @numeroGrupo int, @codigoCurso varchar(20)
AS
BEGIN
	select e.nombre, e.fechaFin, e.porcentaje, e.archivo, e.cantPersonas from Evaluaciones as e
END;
GO


--........................................................TRIGGERS........................................................
--Asigna la misma calificacion a todos los miembros de una evaluacion grupal
CREATE OR ALTER TRIGGER tr_modificacionGrupal on EvaluacionesEstudiantes
for update
AS
BEGIN
	IF ((select grupo from inserted as i) != 0)
	BEGIN
				DECLARE @nota decimal (5,2) = (select nota from inserted);
				update EvaluacionesEstudiantes set 
				nota = (select nota from inserted),
				comentario = (select comentario from inserted),
				archivoRetroalimentacion = (select archivoRetroalimentacion from inserted),
				archivoSolucion = (select archivoSolucion from inserted)
				where idEvaluacion = (select top(1) idEvaluacion from inserted) and grupo = (select grupo from inserted);
	END;
END;
GO

--Trigger que valida que todas las notas de una evaluacion hayan sido asignadas anter de publicar la noticia
CREATE OR ALTER TRIGGER tr_noticiaNotasPublicadas on Evaluaciones
after update
AS
BEGIN
	IF update (revisado)
	DECLARE @idEvaluacion int = (select idEvaluacion from inserted);
	DECLARE @cantidad int = (Select count (*) from EvaluacionesEstudiantes where nota is null and idEvaluacion = @idEvaluacion);
	BEGIN
		IF (@cantidad = 0)
		BEGIN
			IF((select revisado from Evaluaciones where idEvaluacion = @idEvaluacion) = 1)
			BEGIN
				DECLARE @nombEv varchar (50) = (select nombre from inserted);
				DECLARE @titNot varchar (50) = (select CONCAT ('Notas de ', @nombEv));
				DECLARE @mensNot varchar (200) = (select CONCAT ('Las notas de la evaluacion ', @nombEv, ' ya se encuentran disponibles'));
				DECLARE @idRubro int = (select idRubro from inserted);
				DECLARE @idGrupo int = (select idGrupo from Rubros where idRubro = @idRubro);
				DECLARE @autor varchar(20) = (select top(1) cedulaProfesor from ProfesoresGrupo where idGrupo = @idGrupo);
				DECLARE @codCurs varchar(30) = (select codigoCurso from Grupo where idGrupo = @idGrupo);
				DECLARE @numGrup int = (select numeroGrupo from Grupo where idGrupo = @idGrupo);
				Execute crearNoticiaGrupo @codigoCurso = @codCurs, @numeroGrupo = @numGrup, @tituloNoticia = @titNot, @mensaje = @mensNot, @cedulaAutor = @autor;
			END;
		END;
		Else if (@cantidad > 0)
		BEGIN
			RAISERROR ('No se pueden publicar las notas si faltan evaluaciones por calificar',16,1);
			ROLLBACK TRANSACTION;
			Return;
		END;
	END;
END;
GO

--Valida que no se puedan modificar carpetas principales o que se trate de cambiar el nombre por una ya existente
Create or Alter Trigger tr_ActualizarCarpetas on Carpetas
for update
As
IF Update (nombre)
	BEGIN
	Declare @nombreCarpeta varchar(50) = (select nombre from deleted);
	If (@nombreCarpeta = 'Quices' or @nombreCarpeta = 'Examenes' or @nombreCarpeta = 'Proyectos' or @nombreCarpeta = 'Presentaciones')
	BEGIN
		RAISERROR ('No se pueden alterar las carpetas principales',16,1);
		ROLLBACK TRANSACTION;
		Return;
	END;
	Else if Exists (select * from Carpetas as c join inserted as i on c.nombre = i.nombre and c.idGrupo = i.idGrupo having COUNT(*)>1)
	BEGIN
		RAISERROR ('Ya existe una carpeta con ese nombre en el grupo',16,1);
		ROLLBACK TRANSACTION;
		Return
	END;
END;
Go

--Valida que no se pueda cambiar el nombre de un documento por uno que ya existe
Create or Alter Trigger tr_ActualizarDocumentos on Documentos
for update
As
If Exists (select * from Documentos as d join inserted as i on d.nombre = i.nombre and d.idCarpeta = i.idCarpeta having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe un documento con ese nombre en el grupo',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go

--Valida el nombre que se quiere modificar
Create or Alter Trigger tr_ActualizarRubros on Rubros
for update
As
DECLARE @nombRubro varchar (50) = (select rubro from deleted);
DECLARE @nombNuev varchar (50) = (select rubro from inserted);
If Exists (select * from Rubros as r join inserted as i on r.rubro = i.rubro and r.idGrupo = i.idGrupo having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe un rubro con ese nombre en el grupo',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
ELSE IF ((@nombRubro = 'Quices' and @nombNuev != 'Quices') or (@nombRubro = 'Examenes' and @nombNuev != 'Examenes') 
or (@nombRubro = 'Proyectos' and @nombNuev != 'Proyectos'))
BEGIN
	RAISERROR ('No se puede cambiar el nombre de los rubros principales, solo su porcentaje',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go

--Valida el nombre que se quiere modificar de la evaluacion
Create or Alter Trigger tr_ActualizarEvaluaciones on Evaluaciones
for update
As
DECLARE @fechaFin datetime = (select fechaFin from inserted);
DECLARE @fechaInicio datetime = (select fechaInicio from inserted);
If Exists (select * from Evaluaciones as e join inserted as i on e.nombre = i.nombre and e.idRubro = i.idRubro having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe una evaluacion con ese nombre en el grupo',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
ELSE IF (@fechaFin <= @fechaInicio) 
BEGIN
	RAISERROR ('La fecha de finalizacion debe de ser despues de la fecha de inicio de la prueba',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
Go

--Verifica el titulo de la noticia
CREATE OR ALTER TRIGGER tr_verificarNotica on Noticias
for insert
AS
IF Exists (select * from Noticias as n join inserted as i on n.titulo = i.titulo and n.idGrupo = i.idGrupo having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe una noticia con ese nombre en el grupo',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
GO

--Valida el nombre de la notica que se quiere modificar
CREATE OR ALTER TRIGGER tr_modificarNotica on Noticias
for update
AS
IF Exists (select * from Noticias as n join inserted as i on n.titulo = i.titulo and n.idGrupo = i.idGrupo having COUNT(*)>1)
BEGIN
	RAISERROR ('Ya existe una noticia con ese nombre en el grupo',16,1);
	ROLLBACK TRANSACTION;
	Return
END;
GO

--........................................................TRIGGERS........................................................


--*******************************PROFESOR******************************************



--********************************ESTUDIANTE***************************************************

--Ver los cursos a los que pertenece 
CREATE OR ALTER PROCEDURE verCursosEstudiante @carnet varchar(20)
AS
BEGIN
	Select g.numeroGrupo, c.nombre, c.carrera, c.creditos, s.ano, s.periodo, g.codigoCurso from Curso as c
	inner join Grupo as g on g.codigoCurso = c.codigo
	inner join EstudiantesGrupo as e on e.idGrupo = g.idGrupo
	inner join CursosPorSemestre as cs on cs.codigoCurso = c.codigo
	inner join Semestre as s on s.idSemestre = cs.idSemestre
	where carnetEstudiante = @carnet
	order by s.ano desc, s.periodo desc;
END;
GO

--Enviar evaluaciones
CREATE OR ALTER PROCEDURE subirEvaluacion @nombArch varchar(100), @tipoArch varchar (100) , @archivo varchar(MAX), @idEvaluacion int, @carnet varchar(15)
AS
BEGIN
	update EvaluacionesEstudiantes set archivoSolucion = @archivo, nomArchSol = @nombArch, tipoArchSol = @tipoArch where idEvaluacion = @idEvaluacion and carnet = @carnet;
END;
GO

--Permite ver las notas de los estudiantes segun el grupo al que pertenezca
CREATE OR ALTER PROCEDURE verNotasEstudianteGrupo @carnet varchar(15), @codigoCurso varchar (15), @numeroGrupo int
AS
BEGIN
	DECLARE @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	DECLARE @notaFinal decimal (5,2) = (select sum(notaFinalRubro ) from v_notasFinalesRubros where carnet = @carnet);
	select n.nombreEvaluacion, n.rubro, n.notaObtenida, n.porcentajeObtenido, n.porcentajeEvaluacion, nf.notaFinalRubro, @notaFinal notaFinal
	from v_notasEstudiantes as n
	inner join v_notasFinalesRubros as nf on n.carnet = nf.carnet and n.rubro = nf.rubro
	where n.codigoCurso = @codigoCurso and n.carnet = @carnet and numeroGrupo = @numeroGrupo
	group by n.rubro, nombreEvaluacion, notaObtenida, porcentajeObtenido, porcentajeEvaluacion,notaFinalRubro
END;
GO

--Ver las noticias de un grupo ordenadas por fecha
CREATE OR ALTER PROCEDURE verNoticiasGrupo @codigoCurso varchar(10), @numeroGrupo int
AS
BEGIN
	Declare @idGrupo int = (select idGrupo from Grupo where codigoCurso = @codigoCurso and numeroGrupo = @numeroGrupo);
	Select titulo, mensaje, fecha, cedulaAutor from Noticias 
	where idGrupo = @idGrupo order by fecha desc;
END;
GO

--Ver todas las noticias de un estudiante
CREATE OR ALTER PROCEDURE verTodasNoticiasEstudiante @carnet varchar(20)
AS
BEGIN
	select n.titulo, n.mensaje, n.fecha, n.cedulaAutor, c.nombre, g.codigoCurso, g.numeroGrupo from Noticias as n
	inner join EstudiantesGrupo as eg on eg.idGrupo = n.idGrupo
	inner join Grupo as g on n.idGrupo = g.idGrupo
	inner join Curso as c on g.codigoCurso = c.codigo
	where eg.carnetEstudiante = @carnet
	order by fecha desc;
END;
GO

