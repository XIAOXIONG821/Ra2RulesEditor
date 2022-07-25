
import axios, { AxiosResponse } from "axios";
import { RulesInfoModel } from "../model";

let MyAxios = axios.create({
    baseURL: "http://localhost:5000/Api/Rule",
    timeout: 15000,
});

interface IResultModel<T = any> {
    "statusCode": number,
    "data": T,
    "succeeded": boolean,
    "errors": string,
    "extras": string,
    "timestamp": number
}

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
    (res: AxiosResponse<IResultModel>) => {
        // 正常返回的异常,显示提示信息
        if (res.data.statusCode != 200) {
            console.warn("服务器返回的异常:", res.data);
        }
        //取出 数据
        return res.data.data;
    },
    function (error) {
        // 对响应错误做点什么
        console.error(error);

        return Promise.reject(error);
    }
);


export function GetSettingList() {
    return MyAxios.get<any, RulesInfoModel[]>("GetSettingList")
}
export function GetTypeList() {
    return MyAxios.get<any, RulesInfoModel[]>("GetTypeList")
}
export function GetRuleListByTypeName(typeName: string) {
    return MyAxios.get<any, RulesInfoModel[]>(`GetRuleListByTypeName/${typeName}`)
}

export function GetRuleListBySectionName(sectionName: string) {
    return MyAxios.get<any, RulesInfoModel[]>(`GetRuleListBySectionName/${sectionName}`)
}
export function AddOrUpdateSectionRemark(sectionName: string, remark: string) {
    return MyAxios.post<any, boolean>("AddOrUpdateSectionRemark", {
        keyName: sectionName,
        remark: remark,
    })
}
export function AddOrUpdateRuleInfo(data: RulesInfoModel) {
    return MyAxios.post<any, boolean>("AddOrUpdateRuleInfo", data)
}

export function SearchRuleInfo(keyName: string) {
    return MyAxios.get<any, RulesInfoModel[]>(`SearchRuleInfo/${keyName}`)
}

export function DeleteRule({ sectionName, keyName }: RulesInfoModel) {
    return MyAxios.delete<any, boolean>(`DeleteRule/${sectionName}/${keyName}`)
}
export default MyAxios;