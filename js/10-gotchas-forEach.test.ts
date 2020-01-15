import { forceAsync } from "./utils";

const input = [1, 2];

it("Do await Promise.all() if calls can be concurrent", async () => {
  const results = await Promise.all(
    input.map(async item => {
      const result = await forceAsync(item);
      return result;
    })
  );
  expect(results).toEqual(input);
});

it("Do await sequentially if calls must be sequential", async () => {
  const results = [];
  for (const item of input) {
    const result = await forceAsync(item);
    results.push(result);
  }
  expect(results).toEqual(input);
});

it("Don't await array.forEach()", async () => {
  const results = [];
  await input.forEach(async item => {
    const result = await forceAsync(item);
    results.push(result);
  });
  expect(results).toEqual([]); // You might expect this to equal `input`, but it doesn't.
});

it("Don't await array.map() directly", async () => {
  const results = await input.map(async item => {
    const result = await forceAsync(item);
    return result;
  });
  expect(results).not.toEqual(input); // This will be [Promise, Promise], not `input`.
});
