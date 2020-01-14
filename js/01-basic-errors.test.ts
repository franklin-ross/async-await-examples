import { forceAsync } from "./utils";

const basicAsync = async () => {
  await forceAsync();
  throw Error();
};
const basic = () =>
  forceAsync().then(() => {
    throw Error();
  });

it("async and promise throw equivalent", async () => {
  await expect(basicAsync()).rejects.toEqual(Error());
  await expect(basic()).rejects.toEqual(Error());
});
