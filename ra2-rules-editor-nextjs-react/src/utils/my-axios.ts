
import axios from "axios";
import { RulesInfoModel } from "../model";

let MyAxios = axios.create({
    baseURL: "http://localhost:5000/Editor",
    timeout: 15000,
});

// 请求拦截器
MyAxios.interceptors.request.use(
    function (config) {
        if (config.headers) {
            config.headers["authorization"] = "Bearer ";
        }

        return config;
    },
    function (error) {
        return Promise.reject(error);
    }
);

// 响应拦截器
MyAxios.interceptors.response.use(
    (response) => {
        // 正常返回的异常,显示提示信息
        if (response.status != 200) {
            console.warn("服务器返回的异常:", response);
        }
        //取出 数据
        return response.data;
    },
    function (error) {
        // 对响应错误做点什么
        console.error(error);

        return Promise.reject(error);
    }
);

export function SearchRulesInfo(keyName: string) {
    return MyAxios.get<any, RulesInfoModel[]>("SearchRulesInfo", {
        params: {
            keyName: keyName,
        },
    })
}
export function GetSettingList() {
    return MyAxios.get<any, RulesInfoModel[]>("GetSettingList")
}
export function GetTypesList() {
    return MyAxios.get<any, RulesInfoModel[]>("GetTypesList")
}
export function GetRulesListByTypesName(typesName: string) {
    return MyAxios.get<any, RulesInfoModel[]>("GetRulesListByTypesName", {
        params: {
            typesName: typesName,
        },
    })
}

export function GetRulesListBySectionName(sectionName: string) {
    return MyAxios.get<any, RulesInfoModel[]>("GetRulesListBySectionName", {
        params: {
            sectionName: sectionName,
        },
    })
}
export function AddOrUpdateSectionRemark(sectionName: string, remark: string) {
    return MyAxios.post<any, boolean>("AddOrUpdateSectionRemark", {
        keyName: sectionName,
        remark: remark,
    })
}
export function AddOrUpdateRulesInfo(data: RulesInfoModel) {
    return MyAxios.post<any, boolean>("AddOrUpdateRulesInfo", data)
}

export function DeleteRule({ sectionName, keyName }: RulesInfoModel) {
    console.log(sectionName,keyName);
    
    return MyAxios.post<any, boolean>("DeleteRule", {
        sectionName:sectionName,
        keyName:keyName
    })
}
export default MyAxios;