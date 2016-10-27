(function () {
    var spaApp = angular.module('spaApp', ['ngCookies', 'ngRoute', 'ngTable', 'angular-loading-bar', 'ngAnimate', 'angularify.semantic.dropdown']);
    var LOCALHOST = '../../';

    spaApp.config(['$routeProvider', '$locationProvider', 'cfpLoadingBarProvider', function ($routeProvider, $locationProvider, cfpLoadingBarProvider) {
        var viewBaseTemplate = LOCALHOST + 'Areas/Spa/Template/';
        $routeProvider
            .when('/', {
                templateUrl: viewBaseTemplate + 'main.html',
                controller: 'ProductController'
            })
           .when('/Auction/:id', {               
               templateUrl: viewBaseTemplate + 'auction.html',
               controller: 'ProductController'
            })
            .when('/Profile', {
                templateUrl: viewBaseTemplate + 'profile.html',
                controller: 'ProfileController'
            })
            .when('/test', {
                templateUrl: viewBaseTemplate + 'test.html?v2',
                controller: 'CategoryController'
            })
        $routeProvider.otherwise({
            redirectTo: '/'
        });

        $locationProvider.html5Mode({ enabled: true });
        cfpLoadingBarProvider.parentSelector = '#preloader';
        cfpLoadingBarProvider.spinnerTemplate = '<div class="load"></div>';
    }]);

    spaApp.service("bidOffsetService", ["$http", "$q", function ($http, $q) {
        var _bidOffset;
        var defer = $q.defer();

        var setBidOffset = function () {
            $http({
                method: "GET",
                url: LOCALHOST + "/api/bidOffset",
                responseType: "json"
            }).then(
                    function (responce) {
                        defer.resolve(responce.data);
                    },
                    function (error) {
                        defer.reject(error.status);
                    });

            _bidOffset = defer.promise;
        }

        var getBidOffset = function () {
            return _bidOffset;
        }

        return {
            get: getBidOffset,
            set: setBidOffset
        }

    }])

    spaApp.service('userService', ['$cookies', '$rootScope', function ($cookies, $rootScope) {
        var userService = {};
        userService.user = $cookies.get("User") || null;
        userService.user = JSON.parse(userService.user);

        userService.setUser = function (currUser) {
            this.user = currUser;
            $rootScope.$broadcast("userUpdated");
        }

        userService.getUser = function () {
            return this.user;
        }

        return userService;
    }])

    spaApp.service('auctionService', ['$rootScope', function ($rootScope) {
        var _auction = {};

        var setAuctions = function (auctions) {
            _auction = auctions;
        }

        var getAuctions = function () {
            return _auction;
        }

        return {
            getAuctions: getAuctions,
            setAuctions: setAuctions
        }
    }])

    spaApp.service("categoryService", ["$http", "$q", function ($http, $q) {
            promises = [];

        setAuctions = function(_auctions)
        {
            promises = [];
            angular.forEach(_auctions, function (_auction) {
                var defer = $q.defer();
                $http({
                    method: "GET",
                    url: LOCALHOST + "/api/categories/auctionId?auctionId=" + _auction.name,
                    responseType: "json"
                })
                .then(function (responce) {
                    defer.resolve(responce.data);
                },
                function (error) {
                    defer.reject(error.status);
                });
                promises.push(defer.promise);
            })            
        }

            getCategories = function () {
                return $q.all(promises);
            }

        return {
            setAuctions: setAuctions,
            getCategories: getCategories
        }
    }])

    spaApp.controller("PreloaderController", ['$scope', '$interval', function ($scope, $interval) {
        $scope.check = false;
        $interval(function () {
            var count = $("#preloader").children().length;
            (count > 0) ? $scope.check = true : $scope.check = false;
        }, 100)
    }])

    spaApp.controller("AuctionsController", ['$q','$http', '$scope', 'auctionService', '$element', function ($q, $http, $scope, auctionService, $element) {
        var deferred = $q.defer();
        $http({
            method: "GET",
            url: LOCALHOST + "/api/auctions",
            responseType: "json"
        }).then(
        function (responce) {
            $scope.auctions = responce.data;
            deferred.resolve(responce.data);
        },
        function (error) {
            deferred.reject(error.status);
        })

        auctionService.setAuctions(deferred.promise);
        $($element).dropdown();
    }]);

    spaApp.controller("ProductController", ['$q', '$http', '$scope', 'auctionService', 'NgTableParams', '$filter', 'bidOffsetService', 'userService', '$location', function ($q, $http, $scope ,auctionService, NgTableParams, $filter, bidOffsetService, userService, $location) {
        var path = $location.path();
        var auctions = $q.defer();
        if (path !== "/")
        {
            var arr = [],
                auc = {};
            path = path.split("/");
            auc.location = path.pop();
            path = "./#" + path[1] + "/" + auc.location;
            auc.name = $('.menu').find('a[href="' + path + '"]').find("span").text();
            arr.push(auc);
            auctions.resolve(arr);
            auctions = auctions.promise;
        }
        else
        {
            var auctions = auctionService.getAuctions();
        }

        var promises = [];
        var that = $scope;
        that.bids = [];
        var bidsPromise = $q.defer();

        auctions.then(function (_auctions) {
            var auctions = [];
            angular.forEach(_auctions, function (_auction) {
                auctions.push({ id: _auction.name, title: _auction.name });
                var deffered = $q.defer();
                $http({
                    method: "GET",
                    url: LOCALHOST + "/api/products/auctionId?auctionId=" + _auction.location,
                    responseType: "json"
                }).then(
                function (responce) {
                    var listProducts = {
                        auction: _auction,
                        products: responce.data
                    };

                    deffered.resolve(listProducts);
                },
                function (error) {
                    deffered.reject(error.status);
                });
                promises.push(deffered.promise);
            });
            that.listAuctions = auctions;
        }).
        then(function () {
            $q.all(promises).then(function (values) {
                var products = [],
                    productsForBids = [],
                    product = {},
                    categoriesPromise = [];

                angular.forEach(values, function (value) {
                    var productsWithoutPic = [];
                    angular.forEach(value.products, function (_product) {
                        _product.auction = value.auction.name;
                        products.push(_product);
                        productsWithoutPic.push({
                            categoryId: _product.categoryID,
                            description: _product.description,
                            duration: _product.duration,
                            id: _product.id,
                            name: _product.name,
                            startDate: _product.startDate,
                            startPrice: _product.startPrice,
                            state: _product.state,
                            userId: _product.userId
                        })
                    })
                    productsForBids.push({ Auction: value.auction.name, Products: productsWithoutPic })
                });

                //var bidsPromise = $q.defer();
                $http({
                    method: "POST",
                    url: LOCALHOST + "api/bids/productList",
                    data: productsForBids,
                    withCredentials: true
                }).
               then(function (resp) {
                   //$scope.bids = resp.data;
                   bidsPromise.resolve(resp.data);
               },
               function (error) {
                   bidsPromise.reject(error.status);
               });

                angular.forEach($scope.listAuctions, function (_auction) {
                    var categoryPromise = $q.defer();
                    $http({
                        method: "GET",
                        url: LOCALHOST + "api/categories/auctionId?auctionId=" + _auction.id,
                        responseType: "json"
                    }).then(
                    function (responce) {
                        angular.forEach(responce.data, function (_category) {
                            _category.auction = _auction.id;
                        })
                        categoryPromise.resolve(responce.data);
                    },
                    function (error) {
                        categoryPromise.reject(error.status);
                    });
                    categoriesPromise.push(categoryPromise.promise);
                });

                $scope.products = products;
                var that = $scope;

                //need fix for smaller itarations
                $q.all(categoriesPromise).then(function (values) {
                    angular.forEach(values, function (categories) {
                        angular.forEach(categories, function (category) {
                            angular.forEach(that.products, function (product) {
                                if (product.auction === category.auction && product.categoryID === category.id)
                                    product.categoryID = category.name;
                            })
                        })
                    })
                    that.categories = values;

                    that.productsTable = new NgTableParams({
                        page: 1,
                        count: 10
                    }, {
                        total: products.length,
                        dataset: products
                    });
                });
            })
        });

        bidsPromise.promise.then(function (value) {
            $("select").dropdown();
            var that = $scope;
            that.bids = value;
            $scope.getPrice = function () {
                var bids = this.$parent.$parent.bids;
                var product = this.product;
                var price = product.startPrice;
                var that = this;
                that.renderContent = false;
                Object.keys(bids).forEach(function (key) {
                    if (product.auction.toLowerCase() === key && bids[key].length > 0) {
                        angular.forEach(bids[key], function (bid) {
                            if (bid.productId === product.id && bid.price > product.startPrice) {
                                price = bid.price;
                                product.lastBid = bid.price;
                                product.lastBidUser = bid.userId;

                                var getOffsetBid = function (date) {
                                    var dt = +new Date() - +new Date(date);
                                    dt = Math.round(dt / 1000);
                                    var s = dt % 60;
                                    var m = Math.round(dt / 60) % 60;
                                    var h = Math.round(dt / 3600) % 24;
                                    var d = Math.round(dt / (3600 * 24))

                                    return  "last bid: " + d +"d " + h + "h " + m +"min " + s + "s";
                                }
                                that.lastBidTime = getOffsetBid(bid.dateTime);
                                that.bid = bid;
                                that.renderContent = true;
                            }
                        })
                    }
                });
                return price;
            }

            $scope.getPicture = function (id) {
                return 'data:image/jpeg;base64,' + this.product.picture;          
            }           
        })
        $scope.getAuctions = function (data) {
            var def = $q.defer(),
                auctions = [];
            $scope.$watch('listAuctions', function () {
                if ($scope.listAuctions != undefined)
                {
                    angular.forEach($scope.listAuctions, function (item) {
                        auctions.push(item);
                    })
                }
        });
            def.resolve(auctions);
            return def;
        };

        $scope.modalBid = function () {
            $("." + this.product.id + ".modal." + this.product.auction).modal({
                closable: false,
                onApprove: function () {
                    return false;
                }
            }).modal("show");
        }

        $scope.renderBidImage = function () {
           return (this.product.lastBid === undefined) ? false : true;
         }
        
        $scope.getLastPrice = function () {
            return this.product.lastBid || this.product.startPrice;
        }

        $scope.getMinBid = function () {
           
            var bidOffset = this.$parent.bidOffset;
            var price = this.product.lastBid || this.product.startPrice;
            return bidOffset + price;
        }

        $scope.getBidOffset = function () {
            bidOffsetService.set();
            var bidOffset = bidOffsetService.get();
            bidOffset.then(function (value) {
                $scope.bidOffset = value;
            });
        }

        $scope.authorize = function () {
            return userService.getUser() === null ? false : true;
        }

        $scope.putNewBid = function () {
            var that = this;
            var product = this.product;
            var user = userService.getUser();           
            if(user !== undefined)
            {
                var bid = {
                    id: null,
                    userId: user.Id,
                    productId: product.id,
                    dateTime: null,
                    price: this.userBid
                }
                $http({
                    method: "POST",
                    url: LOCALHOST + "api/bids/auctionId/productId?auctionId=" + product.auction,
                    data: bid,
                    context: this,
                    ignoreLoadingBar: true
                }).then(function (resp) {
                    if(resp.status = 200)
                    {
                        that.product.lastBid = that.userBid;
                        that.product.lastBidUser = user.Id;
                        //that.$apply();
                    }

                })
            }
        }

        $scope.getTime = function (product) {
            var self = this,
                time = 0;
            var id = self.product.id,
                auction = self.product.auction;
            var startDate = +new Date(this.product.startDate);
            var duration = function () {
                var ts = self.product.duration.split(":");
                if (ts.length == 3) {
                    return Date.UTC(1970, 0, 1, ts[0], ts[1], ts[2]);
                }
                else if (ts.length == 4) {
                    return Date.UTC(1970, 0, ts[0], ts[1], ts[2], ts[3]);
                }
            }()
            var dt = startDate + duration - (+new Date);

            if (+new Date > startDate + duration) {
                return "Date time out"
            }
            else {

                $scope.$watch('$viewContentLoaded', function () {
                    var clock = $("." + id + ".clock." + auction).FlipClock(dt / 1000 , {
                        countdown: true
                    });
                });
            }
        }

        $scope.getCategories = function (data) {
            var def = $q.defer(),
                categories = [],
                compleate = false;
            $scope.$watch('categories', function () {
                if ($scope.categories != undefined && compleate === false) {
                    angular.forEach($scope.categories, function (category) {
                        angular.forEach(category, function (item) {
                            var checked = false;
                            for(var i = 0, n = categories.length; i < n; i++)
                            {
                                if (categories[i].id === item.id) {
                                    checked = true;
                                    break;
                                }
                            }
                            if(!checked)
                                categories.push({ id: item.id, title: item.name })
                        })
                    })
                    // it's unbearable
                    angular.forEach(categories, function (_category) {
                        _category.id = _category.title;
                    })
                    compleate = true;
                }
            });
            def.resolve(categories);
            return def;
        };

    }])

    spaApp.controller("LoginController", ['$http', '$scope', 'userService', function ($http, $scope, userService) {        
        $scope.auction = "Auction1";
        var logged = function () {
            $scope.authorize = false;
            $scope.user = userService.getUser();
            var user = userService.getUser();
            if (user !== null) {
                $scope.authorize = true;
                $scope.user = {
                    id: user.Id,
                    name: user.FirstName,
                    lastname: user.LastName
                };
            }
        }
        logged();
        $scope.$on('userUpdated', logged);
        $scope.modal = function () {
            $(".loginModal").modal('show');
            return false;
        }
        $scope.logout = function () {
            $http({
                method: "POST",
                url: LOCALHOST + "/api/login/logOut",
                ignoreLoadingBar: true
            }).
            then(
            function (responce) {
                if (responce.status == 200) {
                    $scope.authorize = false;
                }
                userService.setUser(null);
            },
            function (error) {
                console.log(error);
            })
        }
    }]);

    spaApp.controller("LoginPostController", ['$http', '$cookies', '$scope', 'userService', function ($http, $cookies, $scope, userService) {
        var that = $scope;
        $scope.authorizeUser = function () {
            $http({
                method: "POST",
                url: LOCALHOST + "/api/login/logIn",
                data: this.user,
                withCredentials: true,
                ignoreLoadingBar: true
            }).
            then(
            function (responce) {
                if (responce.status == 200)
                {
                    var user = $cookies.get("User");
                    if (user != undefined) {                       
                        userService.setUser(JSON.parse(user));                       
                        $scope.close()
                    }
                }
            },
            function(error)
            {
                console.log(error);
            })
        };
        $scope.close = function ($event) {
            var that = $(".loginModal").find(".close") || $event.currentTarget;
            var modal = that.parent();
            while (true) {
                if (modal.hasClass("modal")) {
                    modal.modal("hide");
                    break;
                }
                modal = modal.parent();
            }
        }
    }])

    spaApp.controller("CategoryController", ['$http', '$scope', function ($http, $scope) {
        $http({
            method: "GET",
            url: LOCALHOST + "/api/categories/auctionId?auctionId=Auction1",
            responseType: "json"
        })
        .then(function (responce) {
            $scope.categories = responce.data;
        });
    }]);
    
    spaApp.controller("ProfileController", ["$cookies", "$scope","userService", function ($cookies, $scope, userService) {
        $scope.authorize = false;

        var user = userService.getUser(); //$cookies.get("User");
        var modal = $(".product-modal");
        if (modal.length > 1)
            $(modal[0]).remove()
 
        if (user != undefined ) {
            $scope.auction = "Auction1";
            $scope.authorize = true;
            $scope.user = user;
        }

        $scope.showProductModal = function () {
            $(".product-modal").modal({
                closable: false,
                onApprove: function () {
                    return false;
                }
            }).modal('show');
        }
    }])

    spaApp.directive("integer", function () {
        var INTEGER_REGEXP = /^\-?\d+$/;
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$validators.integer = function (modelValue, viewValue) {
                    if (ctrl.$isEmpty(modelValue)) {                     
                        return true;
                    }

                    if (INTEGER_REGEXP.test(viewValue)) {
                        return true;
                    }

                    return false;
                };
            }
        };
    })

    spaApp.directive("time", ["$interval", function ($interval) {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$validators.integer = function (modelValue, viewValue) {
                    var that = ctrl;
                    
                    if (ctrl.$isEmpty(modelValue)) {                       
                        return false;
                    }

                    ctrl.$dirty = true;
                    var value = viewValue || 0;

                    if (value !== 0) {
                        var reverseDate = value.split("/");
                        reverseDate = reverseDate[1] + "/" + reverseDate[0] + "/" + reverseDate[2];

                        if (+new Date(reverseDate) > +new Date())
                            return true;
                    }

                    return false;

                };
            }
        };
    }])

    spaApp.controller("CreateProductController", ["$scope", "$interval", "$http", "$element", "auctionService", "categoryService", "userService", "$q", function ($scope, $interval, $http , $element, auctionService, categoryService, userService, $q) {
        var auctions = auctionService.getAuctions();
        var q = $q;
        auctions.then(function (_auctions) {
            $scope.auctions = _auctions;
        })

        $element.find(".timeCalendar").calendar({
            monthFirst: false,
            formatter: {
                date: function (date, settings) {
                    if (!date) return '';
                    var day = date.getDate();
                    var month = date.getMonth() + 1;
                    var year = date.getFullYear();
                    return day + '/' + month + '/' + year;
                },
                time: function (date, settings) {
                    if (!date) {
                        return '';
                    }
                    settings.ampm = false;
                    var hour = date.getHours();
                    var minute = date.getMinutes();
                    return hour + ':' + (minute < 10 ? '0' : '') + minute;
                }
            }
        })

        $interval(function () {
            if($scope.product !== undefined){
                $scope.product.time = $($('form[name = "productForm"]')[0].elements.timeTo).val();
            }
        }, 1000)

        $element.find("#dateTo").on("change", function () {
            if ($scope.product !== undefined) {
                $scope.product.time = $(this).val();
            }
        })

        $scope.changeCategories = function () {
            categoryService.setAuctions([this.product.auction]);
            var categories = categoryService.getCategories();
            categories.then(function (values) {
                $scope.categories = values[0];
            })
        }

        $scope.uploadPic = function ($event) {
            $($event.currentTarget).next().click();
        }

        $scope.createProduct = function () {
            var image = $('form[name = "productForm"]')[0].elements.image;
            var file = image.files[0];

            var picture = q.defer();

            function imageLoad(pic) {
                if (pic.size > 0 && (pic.type === "image/jpeg" || pic.type === "image/png")) {
                    var fileReader = new FileReader();
                    var fileToLoad = pic;
                    fileReader.onload = function (fileLoadedEvent) {
                        var srcData = fileLoadedEvent.target.result;
                        var file = srcData.split(",")[1];
                        picture.resolve(file);
                    }
                    fileReader.readAsDataURL(fileToLoad);
                }
                else
                    picture.resolve();
                return picture.promise;
            }

            imageLoad(file).then(function (value) {
                $scope.product.image = value;
                
                var reverseTime = $scope.product.time.split("/")
                reverseTime = reverseTime[1] + "/" + reverseTime[0] + "/" + reverseTime[2];

                var duration = +new Date(reverseTime) - (+new Date())

                $scope.product.time = toDuration(duration);

                function toDuration(ms) {
                    var sec = Math.round(ms/1000);
                    var days = 24 * 60 * 60,
                        hours = 60 * 60,
                        minutes = 60

                    var d = Math.floor(sec / days);
                    var h = Math.floor((sec % days) / hours);
                    var m = Math.floor(((sec % days) % hours) / minutes)
                    var s = Math.floor(sec % days % hours % minutes)

                    var duration = (function (arr) {                      
                        for (var i = 0, length = arr.length; i < length; i ++)
                        {
                            if ((i > 0) && (arr[i] < 10))
                                arr[i] = "0" + arr[i];
                        }
                        return arr;
                    })([d, h, m, s])

                    return (duration[0] || "") + "." + duration[1] + ":" + duration[2] + ":" + duration[3];
                }

                var product = {
                    id: "",
                    name: $scope.product.name,
                    description: $scope.product.description,
                    startdate: "",
                    duration: $scope.product.time,
                    state: 0,
                    startPrice: $scope.product.stPrice,
                    categoryID: $scope.product.category.id,
                    picture: $scope.product.image,
                    userId: userService.getUser().Id
                }

                $http({
                    method: "POST",
                    url: LOCALHOST + "api/products/auctionId/categoryId/id?auctionId=" + $scope.product.auction.name,
                    data: product,
                    withCredentials: true
                }).
                then(function () {
                    console.log("uspeh");
                });
             });
        }

    }])

    spaApp.filter('formatDate', function () {
        return function(date)
        {
            var dt = new Date(date);
            return dt.getDate() + "/" + (+dt.getMonth() + 1) + "/" + dt.getFullYear();
        }
    })

    spaApp.filter('timeZone', function () {
        return function (tz) {
            var timezone = tz.split("+").join(" ");
            return timezone;
        }
    })
})()