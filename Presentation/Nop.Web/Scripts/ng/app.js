
var mainApp = angular.module("mainApp", ["checklist-model", 'ui.bootstrap', 'infinite-scroll', 'mega-menu','ngDialog']);



// Example of how to set default values for all dialogs
mainApp.config(['ngDialogProvider', function (ngDialogProvider) {
    ngDialogProvider.setDefaults({
        className: 'ngdialog-theme-default',
        plain: false,
        showClose: true,
        closeByDocument: true,
        closeByEscape: true,
        appendTo: false,
        preCloseCallback: function () {
            console.log('default pre-close callback');
        }
    });
}]);
 

mainApp.controller('InsideCtrl', function ($scope, ngDialog, $http, $sce, productId) {
    $scope.displayprogress = true;
    var res = $http.post("/product/ProductDetailsById", { productId: productId })
   .success(function (data, status, headers, config) {

       $scope.product = data;
       $scope.displayprogress = false;
   })
   .error(function (data) {
       $scope.displayprogress = false;
       console.log("Error !" + data);
   });

    $scope.dialogModel = {
        message: 'message from passed scope'
    };

    $scope.AddToBorrowCart = function (value, carttype) {
        $scope.displayprogress = true;
        console.log(1);
        var res = $http.post("addproducttocart/catalog/" + value + "/" + carttype + "/1")
        .success(function (data, status, headers, config) {

            $scope.borrowcart = data;
            console.log(data);
            console.log($scope.borrowcart);
            $scope.displaynotification = false;
            console.log($scope.borrowcart);
            $scope.displayprogress = true;

            if ($scope.borrowcart.success) {
                $scope.color = "#4bb07a";
                $scope.displaynote = "block";
                $scope.displaynotification = true;
                $scope.successmessage = $sce.trustAsHtml($scope.borrowcart.message);
            } else {
                $scope.displaynote = "block";
                $scope.color = "#e4444c";
                $scope.displaynotification = true;
                $scope.successmessage = $sce.trustAsHtml($scope.borrowcart.message);
            };
            $scope.displayprogress = false;

        })
        .error(function (data) {
            $scope.displayprogress = false;
            console.log("Error !" + data);
        });
    }

    $scope.closenote = function () {
        $scope.displaynotification = false;
    }

    $scope.openSecond = function () {
        ngDialog.open({
            template: '<h3><a href="" ng-click="closeSecond()">Close all by click here!</a></h3>',
            plain: true,
            closeByEscape: false,
            controller: 'SecondModalCtrl'
        });
    };

    $scope.SetDefaultImage = function (product, yt) {

        product.DefaultPictureModel.ImageUrl = yt;

    }
});

mainApp.controller('InsideCtrlAs', function () {
    this.value = 'value from controller';
});

mainApp.directive('showonhoverparent',
   function() {
      return {
         link : function(scope, element, attrs) {
            element.parent().bind('mouseenter', function() {
                element.children().eq(1).show();
            });
            element.parent().bind('mouseleave', function() {
                element.children().eq(1).hide();
            });
       }
   };
});

mainApp.controller('SecondModalCtrl', function ($scope, ngDialog) {
    $scope.closeSecond = function () {
        ngDialog.close();
    };
});

mainApp.controller('ModalInstanceCtrl', function ($scope, $uibModalInstance, items) {

    $scope.items = items;
   // console.log(items);
    $scope.selected = {
        item: $scope.items[0]
    };

    $scope.ok = function () {
        $uibModalInstance.close($scope.selected.item);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});


mainApp.controller("CategoryFilterCtrl", ['$scope', '$rootScope', '$http', 'ngDialog', '$location', '$window', '$uibModal', '$sce', function ($scope, $rootScope, $http, ngDialog,$location, $window, $uibModal, $sce) {
   
      
    $scope.displaycaption = function (name, disp) {
        
        if (disp == 1) {
            console.log('#cap' + name+" show");
            document.getElementById('#cap' + name).style.display = "block";
        } else {
            document.getElementById('#cap' + name).style.display = "none";
        }

    }
    $scope.status = {
        isopen: false
    };

    $scope.items = [];

    $scope.animationsEnabled = true;

    
    $scope.open = function (name, yt) {

        //console.log(yt);
        $scope.items.length = 0;
        $scope.items.push(name);
        $scope.items.push($sce.trustAsResourceUrl(yt.replace("watch?v=", "v/") + '&output=embed'));

        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: 'myModalContent.html',
            controller: 'ModalInstanceCtrl',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        }, function () {
            // console.info('Modal dismissed at: ' + new Date());
        });
    };

    $scope.toggleAnimation = function () {
        $scope.animationsEnabled = !$scope.animationsEnabled;
    };

    $scope.selectedCargoTypes = [];
    if (document.getElementById('categoryid') != null) {
        $scope.CategoryId = document.getElementById('categoryid').value
    }
    else {
        $scope.CategoryId = 1;
        console.log(1);
    }

    if (document.getElementById('orderby') != null)
        $scope.OrderBy = document.getElementById('orderby').value;
    else
        $scope.OrderBy = 0;

    if (document.getElementById('pagesize') != null)
        $scope.PageSize = document.getElementById('pagesize').value;
    else
        $scope.PageSize = 12;

    if (document.getElementById('viewmode') != null)
        $scope.ViewMode = document.getElementById('viewmode').value;
    else
        $scope.ViewMode = 'list';

    if (document.getElementById('pagenumber') != null)
        $scope.PageNumber = document.getElementById('pagenumber').value;
    else
        $scope.PageNumber = 1;
    $scope.Reddit = {
        items: [],
        busy: false,
        after: '',
    };

    $scope.SearchTruckModel = {
        OrderBy: $scope.OrderBy,
        ViewMode: $scope.ViewMode,
        PageSize: $scope.PageSize,
        PageNumber: $scope.PageNumber,
        AvailableCargoTypes: [{}],
        PriceRangeFilter: {},
        SpecificationFilter: {},
        AllowProductSorting: false,
        AllowProductViewModeChanging: false,
        AvailableSortOptions: [],
        AvailableViewModes: [],
        AllowCustomersToSelectPageSize: 0,
        PageSizeOptions: [],
        PagingFilteringContext: [],
        filterSpecs: $scope.filterSpecs,
    };
    $scope.Reddit.busy = true;
    var res = $http.post("/catalog/CategoryFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
           .success(function (data, status, headers, config) {
               $scope.list = data;
               if ($scope.list.TotalRecords != 0) {
                   $scope.Reddit.TotalRecords = $scope.list.TotalRecords;
                   $scope.Reddit.PendingRecords = $scope.list.TotalRecords - $scope.list.Products.length;
                   if ((parseInt($scope.list.Products.length) + parseInt($scope.PageSize)) < $scope.list.TotalRecords){
                       $scope.Reddit.TotalDesc = "Show " + ($scope.list.Products.length + 1) + " - " + (parseInt($scope.list.Products.length) + parseInt($scope.PageSize)) + " Items";
                   } else {
                       $scope.Reddit.TotalDesc = "Show Next Items";
                   }
               } else {
                   $scope.Reddit.TotalDesc = "No products found.";
               }
               $scope.Reddit.busy = false;
               console.log("catfilter " + $scope.list.Products.length);
           })
           .error(function (data) {
               console.log("Error !" + data);
           });

   

    $scope.user1 = {
        roles1: []
    };

   
    $scope.list = {
        Products: [],
    };


    $scope.clearmapsearch = function () {
        $scope.addRowAsyncAsJSON();
    };

    $scope.clearfilters = function () {
        $scope.user.roles.length = 0;
         $scope.addRowAsyncAsJSON();
    }

    $scope.clearweight = function () {
        $scope.addRowAsyncAsJSON();
    }

    $scope.user = {
        roles: []
    };

    $scope.clearprice = function () {
        $scope.addRowAsyncAsJSON();
    }

    $scope.clearcategories = function () {
        //$scope.vehicleTypes.length = 0;
        $scope.user.roles.length = 0;
        $scope.user.roles.length = 0;
        $scope.addRowAsyncAsJSON();

    }

    $scope.nextPage = function () {
        
        console.log('next');
        if ($scope.Reddit.PendingRecords != 0) {
                console.log("busy" + $scope.Reddit.busy);
                $scope.Reddit.busy = true;
                $scope.SearchTruckModel.PageNumber = parseInt($scope.SearchTruckModel.PageNumber) + 1;

                var res = $http.post("/catalog/CategoryFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
                   .success(function (data, status, headers, config) {

                       angular.forEach(data.Products, function (item) {
                           $scope.list.Products.push(item);
                       });
                       // console.log($scope.list);
                       $scope.$apply;
                       $scope.Reddit.busy = false;
                       $scope.Reddit.TotalRecords = $scope.list.TotalRecords;
                       $scope.Reddit.PendingRecords = $scope.list.TotalRecords - $scope.list.Products.length;

                       if ((parseInt($scope.list.Products.length) + parseInt($scope.PageSize)) < $scope.list.TotalRecords) {
                           $scope.Reddit.TotalDesc = "Show " + ($scope.list.Products.length + 1) + " - " + (parseInt($scope.list.Products.length) + parseInt($scope.PageSize)) + " Items";
                       } else {
                           $scope.Reddit.TotalDesc = "Show Next Items";
                       }
                       console.log("catfilter1 " + $scope.list.Products.length);
                   })
                   .error(function (data) {
                       console.log("Error !" + data);
                   });
            } else {
                 $scope.Reddit.TotalDesc = "All products loaded.";
            }
            // $scope.addRowAsyncAsJSON();
    };

    $scope.addRowAsyncAsJSON = function () {

        $scope.SearchTruckModel = {
            OrderBy: $scope.OrderBy,
            ViewMode: $scope.ViewMode,
            PageSize: $scope.PageSize,
            PageNumber: $scope.PageNumber,
            AvailableCargoTypes: [{
            }],
            PriceRangeFilter: {},
            SpecificationFilter: {},
            AllowProductSorting: false,
            AllowProductViewModeChanging: false,
            AvailableSortOptions: [],
            AvailableViewModes: [],
            AllowCustomersToSelectPageSize: 0,
            PageSizeOptions: [],
            PagingFilteringContext: [],
            filterSpecs: '',
        };


        // Writing it to the server
        //

        $scope.SearchTruckModel.filterSpecs = $scope.user.roles.join(",");

        console.log($scope.user.roles);

        var res = $http.post("/catalog/CategoryFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
             .success(function (data, status, headers, config) {
                 $scope.list = data;

                 $scope.Reddit.TotalRecords = $scope.list.TotalRecords;
                 $scope.Reddit.PendingRecords = $scope.list.TotalRecords - $scope.list.Products.length;
                 console.log("catfilter "+ data);
             })
             .error(function (data) {
                 console.log("Error !" + data);
             });


        // Making the fields empty
        //
        $scope.name = '';
        $scope.employees = '';
        $scope.headoffice = '';
    };

    $scope.AddToBorrowCart = function (value, carttype) {
        $scope.displayprogress = true;
        console.log(1);
        var res = $http.post("addproducttocart/catalog/" + value + "/" + carttype + "/1")
        .success(function (data, status, headers, config) {

            $scope.borrowcart = data;
            console.log(data);
            console.log($scope.borrowcart);
            $scope.displaynotification = false;
            console.log($scope.borrowcart);
            $scope.displayprogress = true;

            if ($scope.borrowcart.success) {
                $scope.color = "#4bb07a";
                $scope.displaynote = "block";
                $scope.displaynotification = true;
                $scope.successmessage = $sce.trustAsHtml($scope.borrowcart.message);
            } else {
                $scope.displaynote = "block";
                $scope.color = "#e4444c";
                $scope.displaynotification = true;
                $scope.successmessage = $sce.trustAsHtml($scope.borrowcart.message);
            };
            $scope.displayprogress = false;

        })
        .error(function (data) {
            $scope.displayprogress = false;
            console.log("Error !" + data);
        });
    }

    $scope.closenote = function () {
        $scope.displaynotification = false;
    }
    $rootScope.jsonData = '{"foo": "bar"}';
    $rootScope.theme = 'ngdialog-theme-default';

    $scope.openDefault = function (value) {
        ngDialog.open({
            template: 'firstDialogId',
            controller: 'InsideCtrl',
            resolve: {
                productId: function () {
                    return value
                }
            },
            className: 'ngdialog-theme-default',

        });
    };

    $rootScope.$on('ngDialog.opened', function (e, $dialog) {
        console.log('ngDialog opened: ' + $dialog.attr('id'));
    });

    $rootScope.$on('ngDialog.closed', function (e, $dialog) {
        console.log('ngDialog closed: ' + $dialog.attr('id'));
    });

    $rootScope.$on('ngDialog.closing', function (e, $dialog) {
        console.log('ngDialog closing: ' + $dialog.attr('id'));
    });

    $rootScope.$on('ngDialog.templateLoading', function (e, template) {
        console.log('ngDialog template is loading: ' + template);
    });

    $rootScope.$on('ngDialog.templateLoaded', function (e, template) {
        console.log('ngDialog template loaded: ' + template);
    });


}]);


mainApp.controller("SearchFilterController", ['$scope', '$http', '$location', '$window', function ($scope, $http, $location, $window) {


    $scope.displaycaption = function (name, disp) {
        var ele = angular.element('#cap' + name);
        if (disp == 1) {
            ele.show = true;
        } else {
            ele.show = false;
        }

    }

    var s = $location.search();
    console.log(s);

    $scope.status = {
        isopen: false
    };

    $scope.OrderBy = s[categoryId];

    console.log($scope.OrderBy);

    $scope.toggled = function (open) {
        console.log('Dropdown is now: ', open);
    };

    $scope.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };


    $scope.selectedCargoTypes = [];
    if (document.getElementById('categoryid') != null) {
        $scope.CategoryId = document.getElementById('categoryid').value
    }
    else {
        $scope.CategoryId = 1;
        console.log(1);
    }

    if (document.getElementById('pagesize') != null)
        $scope.PageSize = document.getElementById('pagesize').value;
    else
        $scope.PageSize = 12;

    if (document.getElementById('viewmode') != null)
        $scope.ViewMode = document.getElementById('viewmode').value;
    else
        $scope.ViewMode = 'list';

    if (document.getElementById('pagenumber') != null)
        $scope.PageNumber = document.getElementById('pagenumber').value;
    else
        $scope.PageNumber = 1;

    $scope.SearchTruckModel = {
        OrderBy: $scope.OrderBy,
        ViewMode: $scope.ViewMode,
        PageSize: $scope.PageSize,
        PageNumber: $scope.PageNumber,
        AvailableCargoTypes: [{}],
        PriceRangeFilter: {},
        SpecificationFilter: {},
        AllowProductSorting: false,
        AllowProductViewModeChanging: false,
        AvailableSortOptions: [],
        AvailableViewModes: [],
        AllowCustomersToSelectPageSize: 0,
        PageSizeOptions: [],
        PagingFilteringContext: [],
        filterSpecs: '',
    };

    var res = $http.post("/catalog/SearchFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
           .success(function (data, status, headers, config) {
               $scope.list = data;
               console.log(data);
           })
           .error(function (data) {
               console.log("Error !" + data);
           });

    $scope.user = {
        roles: []
    };

    $scope.user1 = {
        roles1: []
    };

    $scope.Reddit = {
        items: [],
        busy: false,
        after: '',
    };
    $scope.list = {
        Products: [],
    };


    $scope.clearmapsearch = function () {
        $scope.addRowAsyncAsJSON();
    };

    $scope.clearfilters = function () {
        $scope.user.roles.length = 0;
        $scope.addRowAsyncAsJSON();

    }

    $scope.clearweight = function () {
        $scope.addRowAsyncAsJSON();
    }

    $scope.user = {
        roles: []
    };


    $scope.clearprice = function () {
        $scope.addRowAsyncAsJSON();
    }

    $scope.clearcategories = function () {
        //$scope.vehicleTypes.length = 0;
        $scope.user.roles.length = 0;
        $scope.user.roles.length = 0;
        $scope.addRowAsyncAsJSON();

    }

    $scope.nextPage = function (i) {
        if (i > 0) {
            console.log("busy" + $scope.Reddit.busy);
            $scope.Reddit.busy = true;
            $scope.SearchTruckModel.PageNumber = parseInt($scope.SearchTruckModel.PageNumber) + 1;

            var res = $http.post("/catalog/CategoryFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
               .success(function (data, status, headers, config) {

                   angular.forEach(data.Products, function (item) {
                       $scope.list.Products.push(item);
                   });
                   // console.log($scope.list);
                   $scope.$apply;
                   $scope.Reddit.busy = false;
               })
               .error(function (data) {
                   console.log("Error !" + data);
               });
            // $scope.addRowAsyncAsJSON();
        }
    };

    $scope.addRowAsyncAsJSON = function () {

       // console.log($scope.CategoryId);

        $scope.SearchTruckModel = {
            OrderBy: $scope.OrderBy,
            ViewMode: $scope.ViewMode,
            PageSize: $scope.PageSize,
            PageNumber: $scope.PageNumber,
            AvailableCargoTypes: [{
            }],
            PriceRangeFilter: {},
            SpecificationFilter: {},
            AllowProductSorting: false,
            AllowProductViewModeChanging: false,
            AvailableSortOptions: [],
            AvailableViewModes: [],
            AllowCustomersToSelectPageSize: 0,
            PageSizeOptions: [],
            PagingFilteringContext: [],
            filterSpecs: '',
            FromPrice: $scope.FromPrice,
            ToPrice: $scope.ToPrice
        };


        // Writing it to the server
        //

        $scope.SearchTruckModel.filterSpecs = $scope.user.roles.join(",");

        console.log($scope.user.roles);

        var searchstring = ''

        searchstring =   searchstring + '&categoryId=' + $scope.CategoryId;


        if ($scope.FromPrice > 0) {
            searchstring =   searchstring + '&price=' + $scope.FromPrice + '-2500'
        }

        searchstring =   searchstring + '&orderby=' + $scope.OrderBy;

        var res = $http.post("/catalog/CategoryFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
             .success(function (data, status, headers, config) {
                 $scope.list = data;
                 alert(123);
                 console.log(data);
                 $scope.list.repeatSelect = $scope.CategoryId
                 $window.location.href = '/SearchProducts?categoryid=' + $scope.CategoryId + ' #!?' + searchstring;
               
             })
             .error(function (data) {
                 console.log("Error !" + data);
             });


        // Making the fields empty
        //
        $scope.name = '';
        $scope.employees = '';
        $scope.headoffice = '';
    };


    $scope.AddToBorrowCart = function (value, carttype) {
        $scope.displayprogress = true;
        console.log(1);
        var res = $http.post("addproducttocart/catalog/" + value + "/" + carttype + "/1")
        .success(function (data, status, headers, config) {

            $scope.borrowcart = data;
            console.log(data);
            console.log($scope.borrowcart);
            $scope.displaynotification = false;
            console.log($scope.borrowcart);
            $scope.displayprogress = true;

            if ($scope.borrowcart.success) {
                $scope.color = "#4bb07a";
                $scope.displaynote = "block";
                $scope.displaynotification = true;
                $scope.successmessage = $sce.trustAsHtml($scope.borrowcart.message);
            } else {
                $scope.displaynote = "block";
                $scope.color = "#e4444c";
                $scope.displaynotification = true;
                $scope.successmessage = $sce.trustAsHtml($scope.borrowcart.message);
            };
            $scope.displayprogress = false;

        })
        .error(function (data) {
            $scope.displayprogress = false;
            console.log("Error !" + data);
        });
    }
    $scope.closenote = function () {
        $scope.displaynotification = false;
    }
}]);


mainApp.controller("SearchControlController", ['$scope', '$http', '$location', '$window', function ($scope, $http,$location, $window) {

    $scope.displaycaption = function (name, disp) {
        var ele = angular.element('#cap' + name);
        if (disp == 1) {
            ele.show = true;
        } else {
            ele.show = false;
        }

    }
    var s = $location.search();
     console.log(s);

    $scope.status = {
        isopen: false
    };

    $scope.SearchTruckModel = {
        CategoryId: $scope.CategoryId,
        OrderBy: $scope.OrderBy,
        ViewMode: $scope.ViewMode,
        PageSize: $scope.PageSize,
        PageNumber: $scope.PageNumber,
        AvailableCargoTypes: [{}],
        PriceRangeFilter: {},
        SpecificationFilter: {},
        AllowProductSorting: false,
        AllowProductViewModeChanging: false,
        AvailableSortOptions: [],
        AvailableViewModes: [],
        AllowCustomersToSelectPageSize: 0,
        PageSizeOptions: [],
        PagingFilteringContext: [],
        filterSpecs: '',
    };

    $scope.toggled = function (open) {
        console.log('Dropdown is now: ', open);
    };

    $scope.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };

    $scope.selectedCargoTypes = [];
    if (document.getElementById('categoryid') != null) {
        $scope.SearchTruckModel.CategoryId = document.getElementById('categoryid').value
    }
    else {
        $scope.SearchTruckModel.CategoryId = 1;
        console.log(1);
    }

    if (document.getElementById('orderby') != null)
        $scope.OrderBy = document.getElementById('orderby').value;
    else
        $scope.OrderBy = 0;

    if (document.getElementById('pagesize') != null)
        $scope.PageSize = document.getElementById('pagesize').value;
    else
        $scope.PageSize = 12;

    if (document.getElementById('viewmode') != null)
        $scope.ViewMode = document.getElementById('viewmode').value;
    else
        $scope.ViewMode = 'list';

    if (document.getElementById('pagenumber') != null)
        $scope.PageNumber = document.getElementById('pagenumber').value;
    else
        $scope.PageNumber = 1;

    

    var res = $http.post("/catalog/SearchFilter", { command: $scope.SearchTruckModel })
           .success(function (data, status, headers, config) {
               $scope.list = data;
               $scope.CategoryId = $scope.list.AvailableCategories[0].CategoryId;
               console.log(data);
           })
           .error(function (data) {
               console.log("Error !" + data);
           });

    $scope.user = {
        roles: []
    };

    $scope.Reddit = {
        items: [],
        busy: false,
        after: '',
    };
    $scope.list = {
        Products: [],
    };


    $scope.clearmapsearch = function () {
        $scope.addRowAsyncAsJSON();
    };

    $scope.clearfilters = function () {
        $scope.user.roles.length = 0;
        $scope.addRowAsyncAsJSON();

    }

    $scope.clearweight = function () {
        $scope.addRowAsyncAsJSON();
    }

    $scope.user = {
        roles: []
    };


    $scope.clearprice = function () {
        $scope.addRowAsyncAsJSON();
    }

    $scope.clearcategories = function () {
        //$scope.vehicleTypes.length = 0;
        $scope.user.roles.length = 0;
        $scope.user.roles.length = 0;
        $scope.addRowAsyncAsJSON();

    }

    $scope.nextPage = function (i) {
        if (i > 0) {
            console.log("busy" + $scope.Reddit.busy);
            $scope.Reddit.busy = true;
            $scope.SearchTruckModel.PageNumber = parseInt($scope.SearchTruckModel.PageNumber) + 1;

            var res = $http.post("/catalog/CategoryFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
               .success(function (data, status, headers, config) {

                   angular.forEach(data.Products, function (item) {
                       $scope.list.Products.push(item);
                   });
                   // console.log($scope.list);
                   $scope.$apply;
                   $scope.Reddit.busy = false;
               })
               .error(function (data) {
                   console.log("Error !" + data);
               });
            // $scope.addRowAsyncAsJSON();
        }
    };

    $scope.Search = function () {

        // console.log($scope.CategoryId);

        $scope.SearchTruckModel = {
            CategoryId :  $scope.CategoryId,
            OrderBy: $scope.OrderBy,
            ViewMode: $scope.ViewMode,
            PageSize: $scope.PageSize,
            PageNumber: $scope.PageNumber,
            AvailableCargoTypes: [{
            }],
            PriceRangeFilter: {},
            SpecificationFilter: {},
            AllowProductSorting: false,
            AllowProductViewModeChanging: false,
            AvailableSortOptions: [],
            AvailableViewModes: [],
            AllowCustomersToSelectPageSize: 0,
            PageSizeOptions: [],
            PagingFilteringContext: [],
            filterSpecs: '',
            FromPrice: $scope.FromPrice,
            ToPrice: $scope.ToPrice
        };


        // Writing it to the server
        //

        $scope.SearchTruckModel.filterSpecs = $scope.user.roles.join(",");

        console.log($scope.user.roles);

        var searchstring = ''

        searchstring = searchstring + '&categoryid=' + $scope.CategoryId;

        searchstring = searchstring + '&filterby=' + $scope.user.roles.join(",");

        if ($scope.FromPrice > 0) {
            searchstring = searchstring + '&price=' + $scope.FromPrice + '-2500'
        }

        searchstring = searchstring + '&orderby=' + $scope.OrderBy;

        $window.location.href = '/SearchProducts?' + '#!?' + searchstring;
        $window.location.reload();
        // Making the fields empty
        //
        $scope.name = '';
        $scope.employees = '';
        $scope.headoffice = '';
    };

    $scope.ChangeOptions = function () {

        // console.log($scope.CategoryId);

        $scope.SearchTruckModel = {
            CategoryId: $scope.CategoryId,
            OrderBy: $scope.OrderBy,
            ViewMode: $scope.ViewMode,
            PageSize: $scope.PageSize,
            PageNumber: $scope.PageNumber,
            AvailableCargoTypes: [{}],
            PriceRangeFilter: {},
            SpecificationFilter: {},
            AllowProductSorting: false,
            AllowProductViewModeChanging: false,
            AvailableSortOptions: [],
            AvailableViewModes: [],
            AllowCustomersToSelectPageSize: 0,
            PageSizeOptions: [],
            PagingFilteringContext: [],
            filterSpecs: '',
            FromPrice: $scope.FromPrice,
            ToPrice: $scope.ToPrice
        };

        // Writing it to the server
        //

        $scope.SearchTruckModel.filterSpecs = $scope.user.roles.join(",");

        var res = $http.post("/catalog/CategoryFilter", { categoryId: $scope.CategoryId, command: $scope.SearchTruckModel })
            .success(function (data, status, headers, config) {
                $scope.list = data;
                console.log(data);
              //  $scope.list.repeatSelect = $scope.CategoryId
             //   $window.location.href = '/SearchProducts?categoryid=' + $scope.CategoryId + ' #!?' + searchstring;
            })
            .error(function (data) {
                console.log("Error !" + data);
            });

        // Making the fields empty
        //
    };


}]);



