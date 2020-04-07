/** 字符串相关类 */
class _Strings {
    /** 判断字符串是否为 null 或 undefined 或 "" */
    IsNull(string) {
        if (string == null || string == undefined || string == "") return true;
        else return false;
    }
    /** 判断字符串是否为 null 或 undefined 或 仅由空字符串组成 */
    IsNulllOrEmpty(string) {
        if (this.IsNull(string)) return true;
        else {
            if (string.replace(/ /g, '') == '') return true;
            else return false;
        }
    }
    /** 判断字符串是否是手机号 */
    IsPhoneNumber(tel) {
        let reg = /^0?1[3|4|5|6|7|8][0-9]\d{8}$/;
        return reg.test(tel);
    }
    /** 判断字符串是否是整数
     * @param {String} number 
     * @param {Number} [type=0] 0:是否为整数 , 1:是否为正整数 ,2:是否为负整数 , 3:是否为非负整数 , 4:是否为0
     */
    IsInt(number, type = 0) {
        let regPos = /^\d+$/; // 非负整数
        let regNeg = /^\-[1-9][0-9]*$/; // 负整数
        let isPos = regPos.test(number); // 非负整数
        let isNeg = regNeg.test(number); // 负整数
        let isZero = ((isPos || isNeg) && Number.parseInt(number) == 0);
        switch (type) {
            case 0: { return isPos || isNeg; }
            case 1: { return isPos && !isZero; }
            case 2: { return isNeg; }
            case 3: { return isPos; }
            case 4: { return isZero; }
            default: return false;
        }
    }
}

/** 字符串的相关操作 */
const Strings = new _Strings();