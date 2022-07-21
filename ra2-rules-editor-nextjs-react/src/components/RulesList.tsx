import {
  PlusCircleOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { yupResolver } from "@hookform/resolvers/yup";
import { Button, Input, Modal, Popover } from "antd";
import classNames from "classnames";
import { cloneDeep, debounce } from "lodash-es";
import { FC, memo, useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import toast from "react-hot-toast";
import { useQuery, useMutation } from "@tanstack/react-query";
import { FixedSizeGrid } from "react-window";
import { RulesInfoModel, RulesInfoModelValidate } from "../model";
import {
  GetRulesListBySectionName,
  AddOrUpdateRulesInfo,
  DeleteRule,
  SearchRulesInfo,
} from "../utils/my-axios";
import MySkeleton from "./MySkeleton";

let RulesList: FC<{ name?: string; remark?: string; prefixTitle?: string }> = (
  props
) => {
  useEffect(() => {
    if (props.name != undefined) {
      setCurrentSectionName(props.name);
      setModalTitle(
        [props.prefixTitle, props.name, props.remark].join(" ").trim()
      );
    }
  }, [props.name, props.prefixTitle, props.remark]);

  const [currentSectionName, setCurrentSectionName] = useState<string>();

  useEffect(() => {}, []);

  //#region 弹框(新增/修改)
  const [modalOpen, setModalOpen] = useState(false);

  const [modalData, setModalData] = useState<RulesInfoModel>();

  const [modalTitle, setModalTitle] = useState<string>();

  let modalOnSubmit = (v: RulesInfoModel) => {
    addOrUpdateRulesInfoMutation.mutateAsync(v);
  };

  let openAddRule = () => {
    rulesListRowClick(undefined);
  };

  // 打开 新增/修改 的弹窗
  let rulesListRowClick = (item: RulesInfoModel | undefined) => {
    if (currentSectionName == "" || currentSectionName == undefined) {
      toast.error("当前不可使用此功能.");
      return;
    }

    // 展开语法创建一个新的对象, 否则相同的item 第二次对象引用没变,不会触发响应式.
    if (item != undefined) {
      setModalData({ ...item });
    } else {
      setModalData(undefined);
    }
    setModalOpen(true);
  };

  //#endregion

  //#region Query rulesList

  let rulesListQueryKey = useMemo(
    () => ["GetRulesListBySectionName", currentSectionName],
    [currentSectionName]
  );

  // 查询
  const rulesListQuery = useQuery(
    rulesListQueryKey,
    async () => {
      let data = await GetRulesListBySectionName(currentSectionName!);
      return data;
    },
    {
      enabled: currentSectionName != undefined,
    }
  );

  //(服务器) 添加 / 修改
  let addOrUpdateRulesInfoMutation = useMutation(
    (v: RulesInfoModel) =>
      AddOrUpdateRulesInfo({ ...v, sectionName: currentSectionName }),
    {
      onSuccess: (d, v, c) => {
        if (d === true) {
          rulesListQuery.refetch();
        }
      },
    }
  );

  //#endregion

  //#region 搜索
  let handleRulesListSearch = debounce(
    (content: string) => {
      setRulesListSearchStr(content);
    },
    500,
    { maxWait: 2000 }
  );

  const [rulesListSearchStr, setRulesListSearchStr] = useState<string>("");

  let rulesListFilterData = useMemo(() => {
    let content = rulesListSearchStr?.trim()?.toLowerCase() ?? "";

    if (content.length > 0) {
      return rulesListQuery.data?.filter((a) => {
        return (
          a.keyName?.toString().toLowerCase().includes(content) ||
          a.currentValue?.toString()?.toLowerCase()?.includes(content) ||
          a.remark?.toString()?.toLowerCase()?.includes(content)
        );
      });
    }
    return rulesListQuery.data;
  }, [rulesListQuery.data, rulesListSearchStr]);
  //#endregion

  //#region 删除
  let deleteRule = (item: RulesInfoModel | undefined) => {
    if (item?.sectionName == undefined || item?.keyName == undefined) {
      return;
    }

    DeleteRule(item).then((res) => {
      if (res === true) {
        rulesListQuery.refetch();
      }
    });
  };
  //#endregion

  //#region child 弹框

  const [childModalOpen, setChildModalOpen] = useState<boolean>(false);
  const [currentChildName, setCurrentChildName] = useState<string>("");
  const [currentChildRemark, setCurrentChildRemark] = useState<string>("");
  let showChildModal = (name: string, remark: string) => {
    setChildModalOpen(true);
    setCurrentChildName(name);
    setCurrentChildRemark(remark);
  };

  //#endregion
  return (
    <>
      <div className="w-full h-full">
        <div className="flex items-center justify-center py-2 space-x-2 border-b-2">
          <span>{modalTitle}</span>
          <Button icon={<PlusCircleOutlined />} onClick={openAddRule}>
            添加rule
          </Button>
          <Input
            className="w-96"
            placeholder="搜索"
            onChange={(e) => handleRulesListSearch(e.currentTarget.value)}
          />
        </div>
        {rulesListQuery.isFetching && <MySkeleton trCount={4} tdCount={5} />}

        <FixedSizeGrid
          width={1000}
          height={500}
          columnCount={1}
          columnWidth={1000}
          rowCount={rulesListFilterData?.length ?? 0}
          rowHeight={50}
          className="w-full"
          overscanRowCount={20}
        >
          {(cell) => {
            let item = rulesListFilterData?.[cell.rowIndex];
            if (item == undefined) {
              return <></>;
            }
            return (
              <div
                className="flex justify-center w-full border-b"
                style={cell.style}
                key={`${item.keyName}${item.id}`}
              >
                <div className="w-[80px] flex justify-center items-center border-r">
                  {item.id}
                </div>
                <div className="w-[300px] flex justify-center items-center border-r">
                  {[
                    "Primary",
                    "ElitePrimary",
                    "Secondary",
                    "EliteSecondary",
                    "Projectile",
                    "Warhead",
                  ].includes(item.keyName) ? (
                    <>
                      <span
                        onClick={() =>
                          showChildModal(item?.currentValue!, item?.remark!)
                        }
                        className="w-full cursor-pointer bg-[lightgoldenrodyellow] text-center"
                      >
                        {item.keyName}
                      </span>
                    </>
                  ) : (
                    item.keyName
                  )}
                </div>
                <div className="w-[160px] flex justify-center items-center border-r">
                  {(item.currentValue?.length ?? 0) > 40 ? (
                    <Popover title={item.keyName} content={item.currentValue}>
                      <Button>查看</Button>
                    </Popover>
                  ) : (
                    item.currentValue?.toString()
                  )}
                </div>
                <div className="w-[600px] flex justify-center items-center border-r">
                  {(item.remark?.length ?? 0) > 40 ? (
                    <Popover title={item.keyName} content={item.remark}>
                      <Button>查看</Button>
                    </Popover>
                  ) : (
                    item.remark
                  )}
                </div>
                <div className="w-[100px] flex justify-center items-center border-r">
                  <div className="flex space-x-2">
                    <Button
                      icon={<EditOutlined />}
                      onClick={() => rulesListRowClick(item)}
                    ></Button>
                    <Button
                      icon={<DeleteOutlined />}
                      danger
                      onClick={() => deleteRule(item)}
                    ></Button>
                  </div>
                </div>
              </div>
            );
          }}
        </FixedSizeGrid>
      </div>

      <ModalForm
        modalTitle={modalTitle}
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
        }}
        formData={modalData}
        onSubmit={modalOnSubmit}
      />

      <Modal
        visible={childModalOpen}
        footer={null}
        width="100%"
        destroyOnClose
        onCancel={() => {
          setChildModalOpen(false);
        }}
      >
        {childModalOpen && (
          <RulesList
            name={currentChildName}
            remark={currentChildRemark}
            prefixTitle={modalTitle}
          />
        )}
      </Modal>
    </>
  );
};

// 表单弹窗 (添加/修改)
let ModalForm: FC<{
  isOpen: boolean;
  onClose: () => void;
  formData: RulesInfoModel | undefined;
  onSubmit: (value: RulesInfoModel) => void;
  modalTitle: string | undefined;
}> = (props) => {
  // 表单验证
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = useForm<RulesInfoModel>({
    mode: "all",
    resolver: yupResolver(RulesInfoModelValidate.pick(["keyName"])),
  });

  // 反填数据
  useEffect(() => {
    reset();
    let data = props.formData;
    if (data != undefined) {
      // react-hooks-form 没有提供 对象赋值,只能自己循环了.
      for (let key of Object.keys(data)) {
        //@ts-ignore
        setValue(key, data[key]);
      }
    }
  }, [props.formData, reset, setValue]);

  const [searchList, setSearchList] = useState<RulesInfoModel[]>();
  const [axiosState, setAxiosState] = useState<boolean>(false);

  // 清空搜索列表
  let clearSearchList = () => {
    setSearchList([]);
  };

  // 智能提示编辑框 输入
  let handleSearch = debounce(
    (currentValue: string) => {
      // 检查内容的有效性
      if (currentValue.replace(/\s*/g, "") == "") {
        clearSearchList();
        return;
      }

      if (axiosState == false) {
        setAxiosState(true);
        SearchRulesInfo(currentValue).then((r) => {
          setSearchList(r);
          setAxiosState(false);
        });
      }
    },
    1000,
    { maxWait: 2000 }
  );

  // 提交
  let onSubmit = (data: RulesInfoModel) => {
    props.onSubmit(cloneDeep(data));
    props.onClose();
  };

  return (
    <>
      <Modal
        visible={props.isOpen}
        footer={null}
        onCancel={() => {
          clearSearchList();
          props.onClose();
        }}
      >
        <form onSubmit={handleSubmit(onSubmit)}>
          <span className="text-sm">正在操作 【{props.modalTitle}】</span>

          <div className="flex flex-col py-4">
            <div className="w-full max-w-xs form-control">
              <label className="label">
                <span className="label-text">
                  名称 <span className="text-red-500">*</span>
                </span>
                <span className="text-red-500 label-text-alt">
                  {errors.keyName?.message}
                </span>
              </label>

              <div className="relative z-50 w-72">
                <input
                  {...register("keyName")}
                  type="text"
                  className={classNames(
                    "input input-bordered w-full max-w-xs",
                    {
                      "input-error": errors.keyName?.message != null,
                    }
                  )}
                  autoComplete="off"
                  onInput={(e) => handleSearch(e.currentTarget.value)}
                />

                {(searchList?.length ?? 0) > 0 && (
                  <>
                    <label
                      onClick={() => {
                        clearSearchList();
                      }}
                      className="absolute btn-sm btn btn-circle -right-9 top-14"
                    >
                      X
                    </label>
                    <div className="absolute w-full overflow-y-auto bg-white border border-red-600 top-14 h-52">
                      {searchList?.map((item) => {
                        return (
                          <p
                            key={item.id}
                            onClick={() => {
                              // 设置选择的名称与相应的备注
                              setValue("keyName", item.keyName);
                              setValue("remark", item.remark);
                              clearSearchList();
                            }}
                            className="hover:bg-gray-200 "
                          >
                            {item.keyName}{" "}
                            <span className="text-xs text-orange-500">
                              {item.remark?.substring(0, 20)}
                            </span>
                          </p>
                        );
                      })}
                    </div>
                  </>
                )}
              </div>
            </div>

            <div className="w-full max-w-xs form-control">
              <label className="label">
                <span className="label-text">当前值</span>
              </label>
              <input
                {...register("currentValue")}
                className="w-full max-w-xs input input-bordered"
                type="text"
              />
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text">备注</span>
              </label>

              <textarea
                {...register("remark")}
                className="h-24 textarea textarea-bordered"
              />
            </div>
          </div>
          <div className="space-x-2 text-right">
            <Button onClick={props.onClose}>关闭</Button>
            <Button htmlType="submit" type="primary">
              确定
            </Button>
          </div>
        </form>
      </Modal>
    </>
  );
};

export default memo(RulesList);
