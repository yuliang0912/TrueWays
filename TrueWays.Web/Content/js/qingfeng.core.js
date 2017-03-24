(function ($) {
    var core = window.$core || {};

    core.ajax = {
        isPreventRequest: false, //是否阻止其他post请求
        getJSON: function (url, postData, successFunc, errorFunc, cache, async) {
            return $.ajax({
                type: "get",
                url: url,
                contentType: "application/json; charset=utf8",
                dataType: "json",
                data: postData,
                cache: cache == undefined ? false : cache,
                async: async == undefined ? true : async,
                success: function (result) {
                    if (result && result.hasOwnProperty("ret") && result.hasOwnProperty("errcode")) {
                        if (result.ret === 2 && result.errcode === -1) {
                            if (parent) {
                                parent.location.href = "/login.html";
                            } else {
                                location.href = "/login.html";
                            }
                        }
                        successFunc && successFunc.call(result, result.data == undefined ? result : result.data);
                    } else {
                        successFunc && successFunc.call(result, result);
                    }
                },
                error: errorFunc || function () {
                    layer.msg("请求数据失败.");
                }
            });
        },
        postJSON: function (url, postData, successFunc, async, beforeSend) {
            if (core.ajax.isPreventRequest) {
                return null;
            }
            return $.ajax({
                type: "post",
                url: url,
                data: postData,
                async: async == undefined ? true : async,
                beforeSend: beforeSend,
                success: function (result) {
                    core.ajax.isPreventRequest = false;
                    if (result && result.hasOwnProperty("ret") && result.hasOwnProperty("errcode")) {
                        if (result.ret === 2 && result.errcode === -1) {
                            if (parent) {
                                parent.location.href = "/login.html";
                            } else {
                                location.href = "/login.html";
                            }
                        }
                        if (result.ret === 0 && result.errcode !== 0) {
                            layer.msg(result.msg || "出错啦");
                        } else {
                            successFunc && successFunc.call(result, result.data == undefined ? result : result.data);
                        }
                    } else {
                        successFunc && successFunc.call(result, result);
                    }
                },
                error: function () {
                    core.ajax.isPreventRequest = false;
                    layer.msg("请求数据失败.");
                }
            });
        }
    };

    core.clipboardData = function (text) {
        if (window.clipboardData) {
            window.clipboardData.setData("text", text) ? alert("复制成功。") : alert("复制失败。");
        } else {
            alert("您使用的浏览器不支持此复制功能，请使用Ctrl+C或鼠标右键。");
        }
    };

    core.changeUrlArg = function (url, arg, argVal) {
        var pattern = arg + '=([^&]*)';
        var replaceText = arg + '=' + argVal;
        if (url.match(pattern)) {
            var tmp = '/(' + arg + '=)([^&]*)/gi';
            tmp = url.replace(eval(tmp), replaceText);
            return tmp;
        } else {
            if (url.match('[\?]')) {
                return url + '&' + replaceText;
            } else {
                return url + '?' + replaceText;
            }
        }
    }

    core.domainHostArray = ["127.0.0.1:1201", "127.0.0.1:1202", "127.0.0.1:1203"];
    core.agentTypeArray = [{id: 1, value: "范围"}, {id: 2, value: "指定"}];
    core.authorize = [
        {key: "oauth", value: "oauth2.0"},
        {key: "basice", value: "basice authorize"},
        {key: "hmacSign", value: "hmac签名"},
        {key: "whiteList", value: "白名单"},
        {key: "blackList", value: "黑名单"}
    ];

    core.validate = {
        isIp4: function (ipAddress) {
            return /^(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})(\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})){3}$/.test(ipAddress)
        },
        isIpWithPort: function (ipAddress) {
            return /^(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})(\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})){3}\:([0-9]|[1-9]\d{1,3}|[1-5]\d{4}|6[0-5]{2}[0-3][0-5])$/.test(ipAddress)
        }
    }

    window.$core = core;
})(jQuery);
;(function () {
    var stringExpand = {
        /**
         * 判断字符串是否是以指定的字符为结尾
         * @method {Boolean}
         **/
        endsWith: function (suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        },
        /**
         * 判断字符串是否是以指定的字符为开始
         * @method {Boolean}
         **/
        startsWith: function (suffix) {
            return this.indexOf(suffix) === 0;
        },
        /**
         * 删除字符串中的指定字符，并返回删除之后的新字符串
         * @param {String/Regex} removeValue
         * @method {String}
         **/
        remove: function (removeValue) {
            return (this + "").replace(removeValue, "");
        },
        /**
         * 删除头尾指定字符
         * @param [param Char] char
         * @method {String}
         **/
        trim: function () {
            return trimChars.call(this, Array.prototype.splice.call(arguments));
        },
        /**
         * 删除尾部指定字符
         * @param [param Char] char
         * @method {String}
         **/
        trimEnd: function () {
            return trimChars.call(this, Array.prototype.splice.call(arguments), 2);
        },
        /**
         * 删除头部指定字符
         * @param [param Char] char
         * @method {String}
         **/
        trimStart: function () {
            return trimChars.call(this, Array.prototype.splice.call(arguments), 1);
        },
        /**
         * 将字符串第一个字母改为大写
         *
         **/
        toUpperCaseFirst: function () {
            return this ? this.charAt(0).toUpperCase() + this.substr(1) : "";
        },
        /**
         * Format格式化字符串
         *
         **/
        format: function () {
            var args = Array.prototype.slice.call(arguments);

            return this.replace(/\{(\d+)\}/g,
                function (m, i) {
                    return args[i];
                }
            );
        },
        /**
         * 是否空白字符串
         *
         **/
        isWhiteSpace: function () {
            return !(this.trim().length > 0);
        }
    };

    function trimChars(chars, option) {
        var removeValue = chars.join("");

        //all
        //因为\s\uFEFF\xA0赋值为字符串时会自动发生转变，使变在无效正则，所以这里使用？：分别处理两种情况
        var pattern = removeValue
            ? eval(String.format("/^[{0}]+|[{0}]+$/g", removeValue))
            : /^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g;

        switch (option) {
            //left
            case 1:
                pattern = removeValue
                    ? eval(String.format("/^[{0}]+/g", removeValue))
                    : /^[\s\uFEFF\xA0]+/;
                break;
            //right
            case 2:
                pattern = removeValue
                    ? eval(String.format("/[{0}]+$/g", removeValue))
                    : /[\s\uFEFF\xA0]+$/;
                break;
        }

        return (this + "").replace(pattern, "");
    }

    $.extend(String.prototype, stringExpand);
})();
window.httpRequest = (function () {
    function setQueryValue(url, queryName, value) {
        var reg = new RegExp("(&|\\?)" + queryName.toLowerCase() + "=([^&]*)(&|$)");
        url = url.replace(reg, "");

        var split = "?";
        if (url.indexOf(split) >= 0) {
            split = "&";
        }
        return url + split + queryName + "=" + encodeURIComponent(value);
    }

    function getUrl(url) {
        //注：这里不能使用decodeURIComponent解码url因为有些参数可能传递的就是网址，一担解码后正则匹配就会出错
        //如:1.apsx?returnUrl=/home?id=1&name=2
        return (url || window.location.search.substr(1)).toLowerCase();
    }

    var request = {
        parseQueryString: function (url) {

            var str = getUrl(url).split("?")[1], items = str.split("&");
            var result = {};
            var arr;
            for (var i = 0; i < items.length; i++) {
                arr = items[i].split("=");
                result[arr[0]] = arr[1];
            }
            return result;
        },
        //取获参数值
        getQueryValue: function (queryName, url) {

            var match = getUrl(url).match(new RegExp("(^|[&\?])" + queryName.toLowerCase() + "=([^&]*)(&|$)"));

            return match != null ? decodeURIComponent(match[2]) : null;
        },
        setQueryValue: function () {

            var query = arguments[0];

            if (typeof query === "string") {
                return setQueryValue(getUrl(arguments[2]), query, arguments[1]);
            } else {
                var url = getUrl(arguments[1]);
                for (var name in query) {
                    if (query.hasOwnProperty(name)) {
                        url = setQueryValue(url, name, query[name]);
                    }
                }
                return url;
            }
        }
    };
    return request;
})();