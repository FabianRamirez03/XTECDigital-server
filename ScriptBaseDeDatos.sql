--Creacion de la base de datos
create database baseXTecDigital

--Creacion de las tablas de la base de datos
create table Administrador (
	cedula varchar(20),
	primary key (cedula)
);

create table Semestre (
	idSemestre int identity(1,1),
	ano int not null,
	periodo varchar(10) not null,
	cedulaAdmin varchar(20) not null,
	primary key (idSemestre)
);


create table Curso (
	codigo varchar(20) not null,
	nombre varchar(50) not null unique,
	carrera varchar(50),
	creditos int,
	habilitado bit default 1,
	idAdministrador varchar(20) default '0',
	primary key (codigo)
);

create table CursosPorSemestre (
	idSemestre int not null,
	codigoCurso varchar(20) not null,
	primary key (idSemestre, codigoCurso)
);


create table Estudiantes (
	carnet varchar(15),
	primary key (carnet)
);


create table Profesor(
	cedula varchar(20),
	primary key (cedula)
);

create table Grupo(
	idGrupo int identity(1,1),
	codigoCurso varchar (10) not null,
	numeroGrupo int not null,
	Unique (codigoCurso,numeroGrupo),
	primary key (idGrupo)
);


create table ProfesoresGrupo (
	idGrupo int not null,
	cedulaProfesor varchar(20) not null,
	primary key (idGrupo, cedulaProfesor)
);

create table EstudiantesGrupo(
	carnetEstudiante varchar(15) not null,
	idGrupo int not null,
	primary key (carnetEstudiante, idGrupo)
);

create table Carpetas(
	idCarpeta int identity(1,1),
	nombre varchar(50) not null,
	tamano int default 0,
	fecha datetime default getDate(),
	idGrupo int not null,
	primary key (idCarpeta)
);


create table Documentos(
	idDocumento int identity(1,1),
	nombre varchar (50) not null,
	archivo varchar(max),
	tipoArchivo varchar (20),
	tamano int default 0,
	fechaSubido datetime default getDate(),
	idCarpeta int not null,
	primary key (idDocumento)
);


create table Noticias(
	idNoticia int identity (1,1),
	titulo varchar (50) not null,
	mensaje varchar (max) not null,
	fecha datetime default getDate(),
	idGrupo int not null,
	cedulaAutor varchar (20),
	primary key (idNoticia)
);


create table Rubros(
	idRubro int identity (1,1),
	rubro varchar (50),
	porcentaje decimal(5,2),
	idGrupo int,
	primary key (idRubro)
);


create table Evaluaciones(
	idEvaluacion int identity (1,1),
	grupal bit default 0,
	nombre varchar(50),
	porcentaje decimal(5,2) not null,
	fechaInicio datetime not null,
	fechaFin dateTime not null,
	archivo varchar(max),
	nombreArchivo varchar (100),
	tipoArchivo varchar (100),
	revisado bit default 0,
	idRubro int not null,
	cantPersonas int,
	primary key (idEvaluacion)
);


create table EvaluacionesEstudiantes (
	carnet varchar(15) not null,
	idEvaluacion int not null,
	grupo int default 0,
	nota decimal(5,2),
	comentario varchar (300),
	archivoRetroalimentacion varchar(max),
	nombArchRetr varchar (100),
	tipoArchRetr varchar (100),
	archivoSolucion varchar(max),
	nomArchSol varchar (100),
	tipoArchSol varchar (100),
	primary key (carnet, idEvaluacion)
);

--Modificaciones de tablas
Alter table Semestre
Add constraint FK_Admin
foreign key (cedulaAdmin) references Administrador (cedula);

Alter table Curso
Add constraint FK_IdAdministrador
foreign key (idAdministrador) references Administrador (cedula);

Alter table CursosPorSemestre
Add constraint FK_idSemestrePorCurso
foreign key (idsemestre) references Semestre (idSemestre);

Alter table CursosPorSemestre
Add constraint FK_codigoCursoPorSemestre
foreign key (codigoCurso) references Curso (codigo);

Alter table ProfesoresGrupo
Add constraint FK_idGrupoProfesores
foreign key (idGrupo) references Grupo (idGrupo);

Alter table ProfesoresGrupo
Add constraint FK_cedulaProfesores
foreign key (cedulaProfesor) references Profesor (cedula);

Alter table EstudiantesGrupo
Add constraint FK_carnetEstudianteGrupo
foreign key (carnetEstudiante) references Estudiantes (carnet);

Alter table Carpetas
Add constraint FK_idGrupo
foreign key (idGrupo) references Grupo (idGrupo);

Alter table Documentos
Add constraint FK_idCarpeta
foreign key (idCarpeta) references Carpetas (idCarpeta);

Alter table Noticias
Add constraint FK_idGrupoNoticias
foreign key (idGrupo) references Grupo (idGrupo);

Alter table Rubros
Add constraint FK_idGrupoRubro
foreign key (idGrupo) references Grupo (idGrupo);

Alter table Evaluaciones
Add constraint FK_idRubro
foreign key (idRubro) references Rubros (idRubro);

Alter table EvaluacionesEstudiantes
Add constraint FK_idEvaluacion
foreign key (idEvaluacion) references Evaluaciones (idEvaluacion);

Alter table EvaluacionesEstudiantes
Add constraint FK_idCarnetEvaluacion
foreign key (carnet) references Estudiantes (carnet);
