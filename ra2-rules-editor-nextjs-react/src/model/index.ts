import { InferType, number, object, string } from "yup"
export let RulesInfoModelValidate = object({
    id: number(),
    keyName: string().required("名称必须填写"),
    currentValue: string(),
    remark: string(),
    keyType: number(),
    sectionName: string()
});

// 使用 InferType, 生成 普通的 ts类型定义
export type RulesInfoModel = InferType<typeof RulesInfoModelValidate>;

