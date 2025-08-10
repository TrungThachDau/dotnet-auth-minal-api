"use client";

import React from "react";
import AntdRegistry from "./antd-registry";
import { ConfigProvider } from "antd";
import "antd/dist/reset.css";

export default function Providers({ children }) {
  return (
    <AntdRegistry>
      <ConfigProvider
        theme={{
          token: {
            colorPrimary: "#1677ff",
          },
        }}
      >
        {children}
      </ConfigProvider>
    </AntdRegistry>
  );
}
