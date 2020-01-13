import { forceAsync } from "./utils";

it("async and promise throw equivalent", async () => {
  const basicAsync = async () => {
    await forceAsync();
    throw Error();
  };
  const basic = () =>
    forceAsync().then(() => {
      throw Error();
    });

  await expect(basicAsync()).rejects.toEqual(Error());
  await expect(basic()).rejects.toEqual(Error());
});
