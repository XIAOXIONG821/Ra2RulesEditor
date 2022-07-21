import { useMemo, useState } from "react";
import { FixedSizeGrid } from "react-window";
import {
  AddOrUpdateSectionRemark,
  GetRulesListByTypesName,
  GetSettingList,
  GetTypesList,
} from "../utils/my-axios";
import classNames from "classnames";
import { debounce, cloneDeep } from "lodash-es";

import {
  useQuery,
  useIsFetching,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import produce from "immer";
import { Button, Drawer, Input } from "antd";

import { useRouter } from "next/router";
import MySkeleton from "../components/MySkeleton";
import RulesList from "../components/RulesList";

function Index() {
  const queryClient = useQueryClient();
  const router = useRouter();

  const [currentSectionName, setCurrentSectionName] = useState<string>();

  const [currentTypesName, setCurrentTypesName] = useState<string>();

  const [sidebarOpen, setSidebarOpen] = useState(false);

  // 记录当前点击的侧边栏子项
  const [editCurrentTypesDetailItemName, setEditCurrentTypesDetailItemName] =
    useState("");

  // 全局的 Fetch 状态
  const isFetching = useIsFetching();

  //#region 顶部 Query
  const settingListQuery = useQuery(["GetSettingList"], GetSettingList);

  const typesListQuery = useQuery(["GetTypesList"], GetTypesList);
  //#endregion

  //#region 侧边栏 Query

  let typesDetailListQueryKey = useMemo(
    () => ["GetRulesListByTypesName", currentTypesName],
    [currentTypesName]
  );

  const typesDetailListQuery = useQuery(
    typesDetailListQueryKey,
    async () => {
      let data = await GetRulesListByTypesName(currentTypesName!);
      return data;
    },
    {
      enabled: currentTypesName != undefined,
    }
  );

  const [typesDetailListSearchStr, setTypesDetailListSearchStr] =
    useState<string>("");

  let handleTypesDetailSearch = debounce(
    (content: string) => {
      setTypesDetailListSearchStr(content);
    },
    500,
    { maxWait: 2000 }
  );

  let typesDetailListFilterData = useMemo(() => {
    let content = typesDetailListSearchStr?.trim()?.toLowerCase() ?? "";

    if (content.length > 0) {
      return typesDetailListQuery.data?.filter((a) => {
        return (
          a.keyName?.toString().toLowerCase().includes(content) ||
          // Fix: 读取ini后变为bool (true,false) ,而bool没有toLowerCase方法,导致报错
          a.currentValue?.toString()?.toLowerCase()?.includes(content) ||
          a.remark?.toString()?.toLowerCase()?.includes(content)
        );
      });
    }
    return typesDetailListQuery.data;
  }, [typesDetailListQuery.data, typesDetailListSearchStr]);

  //#endregion

  const [currentSectionRemark, setCurrentSectionRemark] = useState<string>();
  // 顶部 类型 点击
  let typesListClick = (typesName: string) => {
    setSidebarOpen(true);
    setCurrentTypesName(typesName);
  };

  // 侧边栏/顶部 名称 点击
  let typesDetailListClick = (sectionName: string) => {
    setCurrentSectionName(sectionName);

    // React的 setState 是异步的.
    // 这里要直接用只能用  sectionName 不能用 currentSectionName
    let data = typesDetailListQuery.data?.find(
      (a) => a.currentValue == sectionName
    );

    setCurrentSectionRemark(data?.remark);
    // 设置标题
    // setModalTitle([data?.Id, sectionName, data?.Remark].join(" ").trimEnd());

    setSidebarOpen(false);
  };

  // 侧边栏 编辑 备注
  let updateSectionRemarkMutation = useMutation(
    (v: { sectionName: string; remark: string }) =>
      AddOrUpdateSectionRemark(v.sectionName, v.remark),
    {
      onSuccess: (d, v, c) => {
        if (d === true) {
          // 乐观更新本地数据
          queryClient.setQueryData(
            typesDetailListQueryKey,
            produce(cloneDeep(typesDetailListQuery.data), (data) => {
              let currentItem = data?.find(
                (item) => item.currentValue === v.sectionName
              );
              if (currentItem != undefined) {
                currentItem.remark = v.remark;
              }
            })
          );
        }
      },
    }
  );

  return (
    <>
      <div className="w-full h-full">
        {isFetching > 0 && (
          <p className="fixed top-0 left-2">正在获取服务器数据...</p>
        )}

        {/* 头部 */}
        <div className="flex items-center justify-center py-2 space-x-2 border-b-2">
          {settingListQuery.data?.map((item) => {
            return (
              <Button
                key={item.keyName}
                onClick={() => typesDetailListClick(item.keyName)}
              >
                {item.remark}
              </Button>
            );
          })}
          {typesListQuery.data?.map((item) => {
            return (
              <Button
                key={item.keyName}
                onClick={() => typesListClick(item.keyName)}
              >
                {item.remark}
              </Button>
            );
          })}
        </div>
        <RulesList name={currentSectionName} remark={currentSectionRemark} />
      </div>

      {/* 侧边栏 */}
      <Drawer
        placement="left"
        visible={sidebarOpen}
        closeIcon={null}
        title={
          <>
            <Input
              placeholder="搜索"
              onChange={(e) => handleTypesDetailSearch(e.currentTarget.value)}
            />
            {currentTypesName}
            {"   "}
            {typesDetailListQuery.isFetching && "正在更新..."}
          </>
        }
        onClose={() => setSidebarOpen(false)}
      >
        {typesDetailListQuery.isFetching && (
          <MySkeleton trCount={4} tdCount={3} />
        )}

        <FixedSizeGrid
          width={250}
          height={500}
          columnCount={1}
          columnWidth={250}
          rowCount={typesDetailListFilterData?.length ?? 0}
          rowHeight={50}
          className="w-full"
          overscanRowCount={20}
        >
          {(cell) => {
            let item = typesDetailListFilterData?.[cell.rowIndex];
            if (item == undefined) {
              return <></>;
            }
            return (
              <div
                key={`${item.keyName}${item.id}${item.currentValue}`}
                style={cell.style}
                className={classNames("flex w-full border-b justify-center", {
                  "border-l border-r border-t border-red-500":
                    currentSectionName == item.currentValue,
                })}
              >
                <div className="w-[100px] flex items-center justify-center border-r">
                  {item.keyName}
                </div>
                <div
                  className="w-[200px] flex items-center justify-center border-r"
                  onClick={() => typesDetailListClick(item?.currentValue ?? "")}
                >
                  <span className="bg-yellow-200 border">
                    {item.currentValue}
                  </span>
                </div>
                <div
                  className="w-[200px] flex items-center justify-center border-r"
                  onClick={() =>
                    setEditCurrentTypesDetailItemName(item?.currentValue!)
                  }
                >
                  {editCurrentTypesDetailItemName == item.currentValue ? (
                    <Input
                      defaultValue={item.remark}
                      onBlur={(v) => {
                        // react 的 onChange 效果像 onInput ????
                        // 改为 onBlur 和 Solid 的 onChange 一样
                        // 解决了.
                        // 原来 React 的 onChange 不是原生的事件. 而是一堆事件的合并, 俗称合成事件
                        let value = v.currentTarget.value;
                        if (item?.remark == value) {
                          return;
                        }

                        updateSectionRemarkMutation.mutateAsync({
                          sectionName: item?.currentValue!,
                          remark: value ?? "",
                        });

                        setEditCurrentTypesDetailItemName("");
                      }}
                    />
                  ) : (
                    item.remark
                  )}
                </div>
              </div>
            );
          }}
        </FixedSizeGrid>
      </Drawer>
    </>
  );
}

export default Index;
