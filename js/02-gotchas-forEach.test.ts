import { forceAsync } from "./utils";

it("Don't await array.forEach()", async () => {
  const input = [1, 2];
  const results = [];
  await input.forEach(async item => {
    await forceAsync();
    results.push(item);
  });
  expect(results).toEqual(input);
});

it("Do await Promise.all()", async () => {
  const input = [1, 2];
  const results = await Promise.all(
    input.map(async item => {
      await forceAsync();
      return item;
    })
  );
  expect(results).toEqual(input);
});
