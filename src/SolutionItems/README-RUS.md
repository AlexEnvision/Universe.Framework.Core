# Контроль версий    
Для удобства проект нужно клонировать по локальному пути C:\P\Universe.Framework.Core\ (но это совсем не обязательно)
*  master - основная рабочая ветка, изменения в нее попадают только через MergeRequests
*  develop - вторая основная рабочая ветка, изменения в нее попадают также, через MergeRequests

Для каждой задачи разработки/исправления ошибки заводится отдельный бранч.  

*Создается прямо из задачи, необходимо в названии задачи, ошибки, перед созданием указывать в скобках на английском языке не очень длинное наименование бранча.*

## Правила формирования сообщений к комиту
Сообщение может быть многострочным, например:  
#89 [WebApp Auth]: Добавлена авторизация в web-приложении.  
#89 [Common]: Изменены ссылки на проекты.  
#89 [Universe.Algorithm, Tests]: Удален артефакт.  

где:
*  #89 - номер задачи (как правило совпадает с номером бранча) - в gitlab будет превращаться в ссылку на задачу,
  а при наведении мыши покажет название задачи
*  "WebApp Auth:"" название функционала, в рамках которого делается коммит. Обязательно указывается с двоеточием на конце.
*  "Добавлена авторизация в web-приложении." - текст описывающий, что было сделано. Обязательно с точкой в конце, завершающей предложение.
*  Может быть перечислено несколько действий записываемые подобным образом
  н-р "#89 WebApp Auth: Подключен контейнер Unity в проекте WebApp. Подключен контейнер Unity в проекте Core.""

*  [\~] - указываем в начале строки коммита, если мержим файлы вручную (автоматом git сам формирует сообщение). Сообщение должно быть вида:  
    [\~] Merge from develop to 20-build-ef-data-access-layer  
    или  
    [\~] Merge from develop to #20 

# Средства разработки
Для разработки использовать VS 2017, VS2019 дополнительно должны быть установлены:
*  поддержка PowerShell проектов (это устанавливается при устаноке VS)
*  git интегрируемый в студию (это устанавливается при устаноке VS)
*  ReSharper 2016.2.2 или более новый
*  Если после обновления из ветки develop, у проектов слетели References, а в ошибке фигурирует "NuGet", можно сделать следующее:
   на Solution нажать правую кнопку мыши и выбрать и выбрать Restore NuGet Packages. Затем Clean solution, build solution.
*  Чтобы NuGet ресторил сборки автоматически, нужно сделать следующее:
   в меню Tools - Options - NuGet Package Manager выставить 2 галки:
   - Allow NuGet to download missing packages;
   - Automaticall check for missing packages during build in Visual Studio.
*  MSSQL Server 2014 и выше
* Visual Stidio Code 1.42 и выше

# Версии ПО/платформ/ библиотек
*  MSSQL 2014 SP3 (Build 11.0.6607.3) 

# Обратить внимание
*  На кодировку файлов (особенно *.ps1), должна быть UTF-8

# Coding Style
*  По оформлению кода придерживаться настроек решарпера в файле ReSharper.DotSettings
* !Перед коммитом обязательно выполнять реформат измененного кода по схеме ResharperSln (исключением является кодоген и код
 сгененированны T4 шаблоном) 
*  При реформате выбирать профиль ResharperSln для полного реформата, ResharperSln NoSort для классов в которых нельзя изменять порядок
*  Блок catch, если в блоке catch, нет `throw ...;`, то необходимо указать комментарий почему его тут нет, например
 как в примере ниже  
 Если в блоке catch создается новый инстанс ошибки, то обязательно необходимо указать исходную ошибку, или комментарий
 почему исходная ошибка не должна указываться.
```c#
catch (Exception ex) {
    _log.Unexpected(ex);
    //throw; Что бы здесь не произошло, это не должно повлиять на выполнение всего остального
}
````

# Примеры использования ...
## Формирование выборок по конкретным условиям

В проекте задействован арсенал фильтров для более удобного и гибкого создания запросов к БД.
Ниже приведен пример использования:
```c#

   var fieldMapContainer = new FieldMapContainer<TrainsetClassDb> {
                    FieldMap = new Dictionary<string, Expression<Func<TrainsetClassDb, object>>> {
                        {"ClassName", x => x.ClassName},
                    }
                };

    var filters = new List<ConditionConfiguration>();
    foreach (var className in classNamesCollection)
    {
         var filter = new ContainsConfiguration {
			LeftOperand = new FieldArgumentConfiguration {
				Field = new FieldConfiguration {
					SpFieldName = "ClassName",
				}
			},
			RightOperand = new ValueArgumentConfiguration {
				Expression = className
			}
		};
        filters.Add(filter);
    }

    var orFilter = new OrConfiguration {
			Operands = filters
		};

	var result = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
		new GetEntitiesReq {
			FieldMapContainer = fieldMapContainer,
			Filters = new List<ConditionConfiguration> {
				orFilter
			}
		},
		projection: item => new TrainsetItemDto {
			Id = item.Id,
			ClassName = item.ClassName,
			TrainsetItemValue = item.TrainsetItemValue,
			TrainsetClass = item.TrainsetClassId.HasValue
				? new TrainsetClassDto {
					Id = item.TrainsetClass.Id,
					SessionId = item.TrainsetClass.SessionID,
					TrainSet = item.TrainsetClass.TrainsetId.HasValue
						? new TrainsetDto
						{
							Id = item.TrainsetClass.Trainset.Id,
							Name = item.TrainsetClass.Trainset.Name
						} : null
				} : null
		}
	);
````

 где fieldMapContainer - метаинформация для инициализации фильтров. Также можно использовать имеющийся классы с метаинформацией н-р:
                          var fieldMapContainer = new CaseFilterMetaInfo().FieldMapContainer; В них можно добавлять новые условия, если часто используются.
						  В качестве ключа у элемента Dictionary указываем поле сопоставления, которое можно назвать как угодно.
						  В качестве значения у элемента Dictionary указываем Expression который выступает в качестве поля поиска 
						  и может быть даже из других связанных таблиц;

      filters - условия фильтрации, использующие модель фильтров. 
	            Для поиска с помощею такого фильтра, достаточно указать  SpFieldName и Expression.				
				Значение указываем в строковом формате, тип значения определится автомитически
				
				Важно: SpFieldName указываем тот, что есть в метаинформации;

	  orFilter = new OrConfiguration - конфигурация фильтров "ИЛИ" для поиска значений входящих в диапазон значений коллекции;

	  GetQuery<GetEntitiesQuery<,>>().ExecuteAsync( - это асинхронный метод формирующий запрос и
	            который принимает GetEntitiesReq в качестве аргумента, содержащий фильтры.
				Также он принимает выражения, с помощью которых присоединяет таблицы, которые нам необходимы. 
				При этом важно присоединить таблицы, по которым осуществляем выборку (то что указывается в FieldMapContainer<>)

## EntityReqHelper - Построитель запросов для EntityFramework

 Для типовых выборок из таблиц (и связанных с ними) имеются расширения построения запросов из EntityReqHelper.
 Они позволяют использовать "укороченную" запись для наиболее частых операций выборки.
 Примеры вызова приведены ниже:
 ```c#

 var result = scope.GetQuery<GetEntitiesQuery<TrainsetItemDb>>().Execute(
                    EntityReqHelper.GetInTextsReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDto.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetItemDb>("ClassName", x => x.TrainsetClass.ClassName)
                        ),
                        searchfieldName: "ClassName",
                        patternValues: new List<string> { "42" },
                        countOnPage: 30,
                        pageIndex: 1
                    ),
                    item => item.TrainsetClass,
                    item => item.TrainsetClass.Trainset
                );


 var result = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    EntityReqHelper.GetEqReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDto.Id), x => x.Id)
                        ),
                        eqfieldName: nameof(TrainsetItemDto.Id),
                        eqvalue: "42",
                        countOnPage: 30,
                        pageIndex: 1
                    ),
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue
                    }
                );

 var result = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    EntityReqHelper.GetNeInReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDto.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetItemDb>("ClassName", x => x.TrainsetClass.ClassName)
                        ),
                        searchfieldName: "ClassName",
                        antaSearchValues: new List<string> { "2084", "89" },
                        countOnPage: 30,
                        pageIndex: 1
                    ),
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue
                    }
                );

 var result = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    EntityReqHelper.GetInTextsReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDto>(nameof(TrainsetItemDto.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetItemDto>("ClassName+TrainSetItemValue", x => x.TrainsetClass.ClassName + " " + x.TrainsetItemValue)
                        ),
                        searchfieldName: "ClassName+TrainSetItemValue",
                        patternValues: new List<string> { "89 01011001" },
                        countOnPage: 30,
                        pageIndex: 1
                    ),
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        TrainsetItemValue = item.TrainsetItemValue,
                        TrainsetClass = item.TrainsetClassId.HasValue
                            ? new TrainsetClassDto
                            {
                                Id = item.TrainsetClass.Id,
                                ClassName = item.TrainsetClass.ClassName
                            }
                            : null
                    }
                );
				
````

 где GetQuery<GetEntitiesQuery<>>().Execute( - метод формирующий запрос с полным включением связанной сущности.
             Его аргумент GetEntitiesReq формируется при помощи EntityReqHelper. Метаинформация формируется при помощи MetaInfoHelper.FieldMap(),
             а каждый элемент содержащий сопоставление и выражение через MetaInfoHelper.MapRule<>.

       searchfieldName - поле поиска - указывается то, что есть в метаинформации;
       patternValues - значение, по которому осуществляется поиск;
       countOnPage - количество выбранных элементов на странице;
       pageIndex - номер страницы;
       includes - выражения, которые указываются для присодинения таблиц.
	   
	 GetQuery<SelectEntitiesQuery<,>>().Execute( - метод формирующий запрос с включением связанной сущности посредством проекции.
	         Его аргумент GetEntitiesReq также формируется при помощи EntityReqHelper. Метаинформация формируется при помощи MetaInfoHelper.FieldMap(),
             а каждый элемент содержащий сопоставление и выражение через MetaInfoHelper.MapRule<>.

## Batch-процессы - быстрое масс-сохранение больших объемов данных.

Пример использования:
 ```c#
 
	scope.GetCommand<AddEntitiesBatchCommand<TrainsetDb>>()
		.Execute(x => x.Id, trainSetDb)
		.BatchProcess(
			keySelector: x => x.Id,
			parentEntitiesAfterUpdate: scope
				.GetQuery<SelectEntitiesQuery<TrainsetDb, ExchangeBatchEntity>>().Execute(
					EntityReqHelper.GetEqReq(
						MetaInfoHelper.FieldMap(
							MetaInfoHelper.MapRule<ExchangeBatchEntity>(nameof(ExchangeBatchEntity.Id),
								x => x.Id),
							MetaInfoHelper.MapRule<ExchangeBatchEntity>(
								nameof(ExchangeBatchEntity.SessionId), x => x.SessionId)
						),
						eqfieldName: nameof(ExchangeBatchEntity.SessionId),
						eqvalue: sessionId.ToString(),
						allItemsAsOnePage: true
					),
					projection: s => new ExchangeBatchEntity
					{
						Id = s.Id,
						SessionId = s.SessionID
					}).Items,
			entitiesDict: trainSetDb.TrainsetClasses
				.GroupBy(g => g.Trainset)
				.ToDictionary(g => new ExchangeBatchEntity { Id = g.Key.Id, SessionId = g.Key.SessionID },
					x => x.ToList()),
			parentKeySetterFunc: (items, parentItem) =>
			{
				foreach (var item in items)
				{
					item.Trainset.Id = parentItem.Id;
					item.TrainsetId = parentItem.Id;
				}

				return items;
			})
		.BatchProcess(
			keySelector: x => x.Id,
			parentEntitiesAfterUpdate: scope
				.GetQuery<SelectEntitiesQuery<TrainsetClassDb, ExchangeBatchEntity>>().Execute(
					EntityReqHelper.GetEqReq(
						MetaInfoHelper.FieldMap(
							MetaInfoHelper.MapRule<ExchangeBatchEntity>(nameof(ExchangeBatchEntity.Id),
								x => x.Id),
							MetaInfoHelper.MapRule<ExchangeBatchEntity>(
								nameof(ExchangeBatchEntity.SessionId), x => x.SessionId)
						),
						eqfieldName: nameof(ExchangeBatchEntity.SessionId),
						eqvalue: sessionId.ToString(),
						allItemsAsOnePage: true
					),
					projection: s => new ExchangeBatchEntity
					{
						Id = s.Id,
						SessionId = s.SessionID
					}).Items,
			entitiesDict: trainSetDb.TrainsetClasses.SelectMany(x => x.TrainsetsItems)
				.GroupBy(g => g.TrainsetClass)
				.ToDictionary(g => new ExchangeBatchEntity { Id = g.Key.Id, SessionId = g.Key.SessionID },
					x => x.ToList()),
			parentKeySetterFunc: (items, parentItem) =>
			{
				foreach (var item in items)
				{
					item.TrainsetClass.Id = parentItem.Id;
					item.TrainsetClassId = parentItem.Id;
				}

				return items;
			})
		.BatchProcess(
			keySelector: x => x.Id,
			parentEntitiesAfterUpdate: scope
				.GetQuery<SelectEntitiesQuery<TrainsetItemDb, ExchangeBatchEntity>>().Execute(
					EntityReqHelper.GetEqReq(
						MetaInfoHelper.FieldMap(
							MetaInfoHelper.MapRule<ExchangeBatchEntity>(nameof(ExchangeBatchEntity.Id),
								x => x.Id),
							MetaInfoHelper.MapRule<ExchangeBatchEntity>(
								nameof(ExchangeBatchEntity.SessionId), x => x.SessionId)
						),
						eqfieldName: nameof(ExchangeBatchEntity.SessionId),
						eqvalue: sessionId.ToString(),
						allItemsAsOnePage: true
					),
					projection: s => new ExchangeBatchEntity
					{
						Id = s.Id,
						SessionId = s.SessionID
					}).Items,
			entitiesDict: trainSetDb.TrainsetClasses
				.SelectMany(x => x.TrainsetsItems.SelectMany(y => y.Vectors))
				.GroupBy(g => g.TrainsetItem)
				.ToDictionary(g => new ExchangeBatchEntity { Id = g.Key.Id, SessionId = g.Key.SessionID },
					x => x.ToList()),
			parentKeySetterFunc: (items, parentItem) =>
			{
				foreach (var item in items)
				{
					item.TrainsetItem.Id = parentItem.Id;
					item.TrainsetItemId = parentItem.Id;
				}

				return items;
			});
````

# EntityFramework
## Добавление миграций

После изменений моделей в проекте «Universe.DataAccess» необходимо добавлять миграции. 
Для этого в Visual Studio идем в окно «Консоль диспетчера пакетов». В нём выбираем проект по-умолчанию 
в который вносились изменения. Затем командой Add-Migration добавляется новая миграция н-р: Add-Migration InitialDb.
Предварительно перед миграцией лучше выполнить команду Update-database -Verbose.