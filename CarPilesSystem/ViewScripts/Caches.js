/** 缓存项的键 */
class _CacheKey {
    get UserNameKey() { return "cps_username"; }
    get PasswordKey() { return "cps_password"; }
    get AdminUserNameKey() { return "cps_admin_username"; }
    get AdminPasswordKey() { return "cps_admin_password"; }
}
const CacheKey = new _CacheKey();

/**
 * 设置 Cookie
 * @param {any} name
 * @param {any} value
 */
function SetCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
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
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
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

    /**
     * 清除指定的缓存项
     * @param {CacheKey} key 指定项的 key
     */
    Clear(key) { DelCookie(key); }

    /** 清除所有缓存项 */
    Clear() { document.cookie = ""; }
};
/** 缓存类 */
const Caches = new _Caches();