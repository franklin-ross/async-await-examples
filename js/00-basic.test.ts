import { forceAsync } from "./utils";

it("async and promise is equivalent", async () => {
  const withAsync = async () => {
    await forceAsync();
    return 5;
  };
  const withoutAsync = () => forceAsync().then(() => 5);
  await [].forEach(x => {});
  expect(await withAsync()).toEqual(5);
  expect(await withoutAsync()).toEqual(5);
});
