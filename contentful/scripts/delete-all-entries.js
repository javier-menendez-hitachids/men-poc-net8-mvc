const contentful = require('contentful-management')

const client = contentful.createClient({
  accessToken: 'CFPAT-Cn_x4G4dwiVA_HlSuy6u-iOrmABvm3i5VW7yiZ8RxbI'
})

const spaceId = 'o18rrodrm1xn'
const environmentId = 'master' // usually 'master' if not specified

async function deleteAllEntries() {
  try {
    const space = await client.getSpace(spaceId)
    const environment = await space.getEnvironment(environmentId)

    // Get all content types
    const contentTypes = await environment.getContentTypes()

    for (const contentType of contentTypes.items) {
      console.log(`Deleting entries for content type: ${contentType.name}`)

      let hasMoreEntries = true
      while (hasMoreEntries) {
        const entries = await environment.getEntries({
          content_type: contentType.sys.id,
          limit: 1000 // Contentful API limit
        })

        if (entries.items.length > 0) {
          await Promise.all(entries.items.map(entry => entry.unpublish()))
          await Promise.all(entries.items.map(entry => entry.delete()))
          console.log(`Deleted ${entries.items.length} entries`)
        } else {
          hasMoreEntries = false
        }
      }

      console.log(`Finished deleting entries for content type: ${contentType.name}`)
    }

    console.log('All entries in all content types have been deleted')
  } catch (error) {
    console.error('Error:', error)
  }
}

deleteAllEntries()

