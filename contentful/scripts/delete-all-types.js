const contentful = require('contentful-management')

const client = contentful.createClient({
  accessToken: 'CFPAT-Cn_x4G4dwiVA_HlSuy6u-iOrmABvm3i5VW7yiZ8RxbI'
})

const spaceId = 'o18rrodrm1xn'
const environmentId = 'master' // usually 'master' if not specified

async function deleteAllContentTypes() {
  try {
    const space = await client.getSpace(spaceId)
    const environment = await space.getEnvironment(environmentId)

    // Get all content types
    const contentTypes = await environment.getContentTypes()

    for (const contentType of contentTypes.items) {
      console.log(`Deleting content type: ${contentType.name}`)

      // Unpublish the content type if it's published
      if (contentType.isPublished()) {
        await contentType.unpublish()
      }

      // Delete the content type
      await contentType.delete()

      console.log(`Deleted content type: ${contentType.name}`)
    }

    console.log('All content types have been deleted')
  } catch (error) {
    console.error('Error:', error)
  }
}

deleteAllContentTypes()
