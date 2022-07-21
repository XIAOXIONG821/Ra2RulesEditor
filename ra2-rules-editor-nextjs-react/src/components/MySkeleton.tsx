import { Skeleton } from "antd";

function MySkeleton(props: { trCount: number; tdCount: number }) {
    return (
      <div className="m-2 space-y-2 ">
        {[...new Array(props.trCount)].map((item, i) => (
          <div className="flex space-x-2" key={"TrTdSkeleton-tr" + i}>
            {[...new Array(props.tdCount)].map((item, y) => (
              <div className="w-full" key={"TrTdSkeleton-td" + i + "-" + y}>
                <Skeleton.Input active className="w-full min-w-0" />
              </div>
            ))}
          </div>
        ))}
      </div>
    );
  }

  export default MySkeleton