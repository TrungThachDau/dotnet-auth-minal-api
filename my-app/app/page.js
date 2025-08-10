"use server";

import HomeTemplate from "./HomeTemplate";
import TableComponent from "./Table";

export default async function Home() {
  return (
    <HomeTemplate>
      <TableComponent />
    </HomeTemplate>
  );
}
