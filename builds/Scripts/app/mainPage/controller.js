/// <reference path="~/Scripts/angular.js"/>
/// <reference path="~/Scripts/ng-table.js"/>

// Must be defined outside
window.urlCard = window.urlTaskCard ? window.urlTaskCard : "";

(function () {

    // Регистрация модуля
    angular.module("app.EntitiesListPage", [
        "ngAnimate",
        "ui.date",
        "ngTable",
        "app.commonDirectives",
        "app.EntitiesServices"
    ]).controller("TaskController", taskController);

    taskController.$inject = ["$scope", "$location", "CrudService", "UtilService", "ngTableParams"];

    function taskController($scope, $location, CrudService, UtilService, ngTableParams) {

        var viewModel = this;

        viewModel.importanceList = null;
        try {
            UtilService.importanceList()
                .success(function (res) {
                    viewModel.importanceList = res.result;
                })
                .error(function (data) {
                    alert("Ошибка при загрузке данных");
                });
        } catch (e) {
        }

        //----- Свойства ------//
        viewModel.list = null;
        
        viewModel.formData = { };
        viewModel.isTableLoading = false;
        viewModel.searchParams = {};
        viewModel._searchParamsToGrid = {};

        // для диалога (на будущее)
        viewModel.dialogSrc = "";
        viewModel.isDialogOpen = false;

        //----- Методы -----//

        viewModel.searchItems = searchItems;
        viewModel.resetSearch = resetSearch;
        viewModel.reloadGrid = function () { viewModel.tableParams.reload() };

        viewModel.editItem = editItem;
        viewModel.deleteItem = deleteItem;

        viewModel.saveForm = saveForm;

        //----- Параметры таблицы -----//
        viewModel.tableParams = new ngTableParams(
            // параметры
            angular.extend(
            {
                page: 1,
                count: 10,
                sorting: { id: 'desc' },
                filter: viewModel._searchParamsToGrid
            }, $location.search()),
            // настройки
            {
                total: 0,           // length of data

                getData: function ($defer, params) {

                    // Запихиваем параметры в URL
                    $location.search(params.url());
                    angular.extend(viewModel.searchParams, viewModel._searchParamsToGrid); // при обновлении страницы браузера нужно заполнить параметры

                    // Трансформируем параметры
                    var filterObj = params.filter();

                    var sortObj = angular.copy(params.sorting());
                    for (var sort in sortObj) {
                        if (sortObj.hasOwnProperty(sort)) {
                            sortObj[sort] = sortObj[sort] === "asc" ? true : false; // трансформируем
                        }
                    }

                    var take = params.count();
                    var skip = (params.page() - 1) * take;

                    CrudService.list(filterObj, sortObj, skip, take)
                        .success(function (data) {
                            // update table params
                            params.total(data.totalCount);
                            // set new data
                            $defer.resolve(data.result);
                        })
                        .error(function (data) {
                            $defer.reject(data && data.error ? data.error : "no answer");
                            alert("Ошибка при загрузке данных");
                        });
                }
            });


        //----- Private-методы -----//
        function searchItems() {
            _setParamsToGrid(viewModel.searchParams);
        }

        function resetSearch() {
            for (var prop in viewModel.searchParams) {
                if (viewModel.searchParams.hasOwnProperty(prop))
                    viewModel.searchParams[prop] = null;
            }

            _setParamsToGrid(viewModel.searchParams);
        }

        function _setParamsToGrid(searchParams) {
            angular.extend(viewModel._searchParamsToGrid, searchParams);      // релоад происходит автоматом
        }

        function editItem(id) {
            viewModel.dialogSrc = window.urlCard + "/" + id;
            viewModel.isDialogOpen = true;
            $("#cardFrame").prop("src", window.urlCard + "/" + id);
            $("#popupCard").dialog("open");      // это плохо
        }


        function deleteItem(id) {
            if (!confirm("Вы действительно хотите удалить задачу?"))
                return;

            viewModel.isTableLoading = true;
            CrudService.delete(id)
                .success(function () {
                    viewModel.tableParams.reload();
                })
                .error(function (data) {
                    alert("Удаление не удалось" + (data && data.error ? "\n" + data.error : ""));
                })
                .finally(function () {
                    viewModel.isTableLoading = false;
                });
        }

        function saveForm() {
            viewModel.formData.id = 0;
            viewModel.formData.created = new Date();

            CrudService.saveForm(viewModel.formData)
                .success(function (data) {
                })
                .error(function (data) {
                    alert("Ошибка при сохранении данных");
                });

            location.href = window.card.successUrl;
        }

    }


})();