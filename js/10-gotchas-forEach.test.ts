import { forceAsync } from "./utils";

const input = [1, 2];

it("Do await Promise.all() if calls can be concurrent", async () => {
  const results = await Promise.all(
    input.map(async item => {
      await forceAsync();
      return item;
    })
  );
  expect(results).toEqual(input);
});

it("Do await sequentially if calls must be sequential", async () => {
  const results = [];
  for (const item of input) {
    await forceAsync();
    results.push(item);
  }
  expect(results).toEqual(input);
});

it("Don't await array.forEach()", async () => {
  const results = [];
  await input.forEach(async item => {
    await forceAsync();
    results.push(item);
  });
  expect(results).toEqual(input);
});

it("Don't await array.map() directly", async () => {
  const results = await input.map(async item => {
    await forceAsync();
    return item;
  });
  expect(results).toEqual(input);
});
