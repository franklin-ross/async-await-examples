import { forceAsync } from "./utils";

const withAsync = async () => {
  await forceAsync();
  return 5;
};

const withoutAsync = () => forceAsync().then(() => 5);

it("async and promise is equivalent", async () => {
  expect(await withAsync()).toEqual(5);
  expect(await withoutAsync()).toEqual(5);
});
