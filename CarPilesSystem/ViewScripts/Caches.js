/** 缓存项的键 */
class _CacheKey {
    get UserNameKey() { return "cps_username"; }
    get PasswordKey() { return "cps_password"; }
    get AdminUserNameKey() { return "cps_admin_username"; }
    get AdminPasswordKey() { return "cps_admin_password"; }
    get UserIdKey() { return "cps_userid"; }
    get PayIdKey() { return "cps_payid"; }
    get PayMoneyKey() { return "cps_money"; }
}
const CacheKey = new _CacheKey();

/**
 * 设置 Cookie
 * @param {any} name
 * @param {any} value
 */
function SetCookie(name, value) {
    var Days = 1;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    //document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
    let cookie = `${name}=${escape(value)};path=/;expires=${exp.toGMTString()}`;
    document.cookie = cookie;
}

/**
 * 获取 Cookie
 * @param {any} name
 */
function GetCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}

/**
 * 删除 Cookies
 * @param {any} name
 */
function DelCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 10000);
    var cval = getCookie(name);
    if (cval != null) {
        let cookie = `${name}=${cval};path=/;expires=${exp.toGMTString()}`;
        document.cookie = cookie;
    }
}

class _Caches {

    /** 获取或设置缓存中的 UserName */
    get UserName() { return GetCookie(CacheKey.UserNameKey); }
    set UserName(username) { SetCookie(CacheKey.UserNameKey, username); }

    /** 获取或设置缓存中的 Password */
    get Password() { return GetCookie(CacheKey.PasswordKey); }
    set Password(password) { SetCookie(CacheKey.PasswordKey, password); }

    /** 获取或设置管理员账号 */
    get AdminUserName() { return GetCookie(CacheKey.AdminUserNameKey); }
    set AdminUserName(username) { SetCookie(CacheKey.AdminUserNameKey, username); }

    /** 获取或设置管理员密码 */
    get AdminPassword() { return GetCookie(CacheKey.AdminPasswordKey); }
    set AdminPassword(password) { SetCookie(CacheKey.AdminPasswordKey, password); }

    /** 获取或设置用户 Id */
    get UserId() { return GetCookie(CacheKey.UserIdKey); }
    set UserId(userId) { SetCookie(CacheKey.UserIdKey, userId); }

    /** 获取或设置支付订单 Id */
    get PayId() { return GetCookie(CacheKey.PayIdKey); }
    set PayId(payId) { SetCookie(CacheKey.PayIdKey, payId); }

    /** 获取或设置支付金额 */
    get PayMoney() { return GetCookie(CacheKey.PayMoneyKey); }
    set PayMoney(money) { SetCookie(CacheKey.PayMoneyKey, money); }

    /**
     * 清除指定的缓存项
     * @param {CacheKey} key 指定项的 key
     */
    Clear(key) {
        SetCookie(key, "");
        DelCookie(key);
    }

    /** 清除所有缓存项 */
    Clear() { document.cookie = ""; }
};
/** 缓存类 */
const Caches = new _Caches();