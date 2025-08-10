"use client";
import { Layout, Row, Col, Button } from "antd";
const { Header, Content, Footer } = Layout;

export default function HomeTemplate({ children }) {
  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Row justify="space-between" align="middle">
        <Col>
          <h1 style={{ margin: 0 }}>My App</h1>
        </Col>
        <Col>
          <Button type="primary">Action</Button>
        </Col>
      </Row>
      <Content style={{ margin: "24px 16px" }}>
        <Row justify="center" align="top">
          <Col xs={24} sm={20} md={16} lg={12}>
            {children}
          </Col>
        </Row>
      </Content>
      <Footer style={{ textAlign: "center" }}>
        ©2025 My App. All rights reserved.
      </Footer>
    </Layout>
  );
}
