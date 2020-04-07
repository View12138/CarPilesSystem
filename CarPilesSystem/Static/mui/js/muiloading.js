//扩展mui.showLoading  
(function (window) {
    //显示加载框  
    mui.showLoading = function (message, type) {

        var html = '';
        html += '<i class="mui-spinner mui-spinner-white"></i>';
        html += '<p class="text">' + (message || "数据加载中") + '</p>';

        //遮罩层  
        var mask = document.getElementsByClassName("mui-show-loading-mask");
        if (mask.length === 0) {
            mask = document.createElement('div');
            mask.classList.add("mui-show-loading-mask");
            document.body.appendChild(mask);
            mask.addEventListener("touchmove", function (e) { e.stopPropagation(); e.preventDefault(); });
        } else {
            mask[0].classList.remove("mui-show-loading-mask-hidden");
        }
        //加载框  
        var toast = document.getElementsByClassName("mui-show-loading");
        if (toast.length === 0) {
            toast = document.createElement('div');
            toast.classList.add("mui-show-loading");
            toast.classList.add('loading-visible');
            document.body.appendChild(toast);
            toast.innerHTML = html;
            toast.addEventListener("touchmove", function (e) { e.stopPropagation(); e.preventDefault(); });
        } else {
            toast[0].innerHTML = html;
            toast[0].classList.add("loading-visible");
        }
    };

    //隐藏加载框  
    mui.hideLoading = function (callback) {
        var mask = document.getElementsByClassName("mui-show-loading-mask");
        var toast = document.getElementsByClassName("mui-show-loading");
        if (mask.length > 0) {
            mask[0].classList.add("mui-show-loading-mask-hidden");
        }
        if (toast.length > 0) {
            toast[0].classList.remove("loading-visible");
            callback && callback();
        }
    };
})(window);  