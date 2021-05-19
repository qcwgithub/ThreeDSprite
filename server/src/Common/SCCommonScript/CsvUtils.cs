function toValue(string type, string cell) {
    switch (type) {
        case "boolean": {
            return cell == "1" || cell == "true";
        }
        case "string": {
            return cell;
        }
        case "stringEx": {
            var str = cell.split("|").join(",").split("\\n").join("\n");
            return str;
        }
        case "number": {
            if (cell.length == 0) {
                return 0;
            }
            return parseInt(cell);
        }
        case "array": {
            if (cell.length == 0) {
                return [];
            }
            return JSON.parse(cell.split("|").join(","));
        }
        case "object": {
            if (cell.length == 0) {
                return {};
            }
            return JSON.parse(cell.split("|").join(","));
        }
        case "json":
            return JSON.parse(cell.split("|").join(","));
        default:
            throw "unknown field type: " + type;
    }
}

// 简易 csv 解析器，解析成对象数组
// 限制：内容中不可包含英文逗号和英文引号
// 不能勾选【引用字段做为文字】，也就是字符串不要自动加上英文引号
public class CsvUtils {
    public static parseToObjectArray(string text): any[] {
        var string arr[] = text.split(/\r?\n/);
        var types = arr[0].split(",");
        var L = types.length;
        var fields = arr[1].split(",");
        if (fields.length != L) {
            console.error("csv error, fields.length != " + L);
            return null;
        }

        var object ret[] = [];

        for (var i = 2; i < arr.length; i++) {
            if (arr[i].length == 0 || arr[i].startsWith("#")) { // 忽略#号开头的行
                continue;
            }
            var cells = arr[i].split(",");
            if (cells.length != L) {
                console.error("csv error, line " + i + " length != " + L);
                return null;
            }

            var object obj = {};

            for (var j = 0; j < L; j++) {
                obj[fields[j]] = toValue(types[j], cells[j]);
            }

            ret.push(obj);
        }

        return ret;
    }
}